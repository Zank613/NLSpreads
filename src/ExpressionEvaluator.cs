using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NLSpreads
{
    /// <summary>
    /// Parses and evaluates simple arithmetic expressions with cell references (e.g. "A1.B2 + 3 * (C3.D4 - 2)").
    /// Supports +, -, *, / and parentheses.
    /// </summary>
    public static class ExpressionEvaluator
    {
        public static double Evaluate(string expression, SpreadsheetState state)
        {
            var tokens = Tokenize(expression);
            var rpn = ToRpn(tokens);
            return EvaluateRpn(rpn, state);
        }

        private static List<string> Tokenize(string expr)
        {
            // Matches cell refs (Row.Col), numbers, operators and parentheses
            var pattern = @"([A-Za-z0-9_]+\.[A-Za-z0-9_]+)|(\d+(\.\d+)?)|[\+\-\*/\(\)]";
            var matches = Regex.Matches(expr, pattern);
            var list = new List<string>();
            foreach (Match m in matches)
                list.Add(m.Value);
            return list;
        }

        private static List<string> ToRpn(List<string> tokens)
        {
            var output = new List<string>();
            var ops = new Stack<string>();

            int Precedence(string op) => op switch
            {
                "+" or "-" => 1,
                "*" or "/" => 2,
                _ => 0
            };

            foreach (var tok in tokens)
            {
                // Number or cell reference
                if (Regex.IsMatch(tok, @"^[A-Za-z0-9_]+\.[A-Za-z0-9_]+$") || double.TryParse(tok, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                {
                    output.Add(tok);
                }
                else if (tok == "(")
                {
                    ops.Push(tok);
                }
                else if (tok == ")")
                {
                    while (ops.Count > 0 && ops.Peek() != "(")
                        output.Add(ops.Pop());

                    if (ops.Count == 0)
                        throw new Exception("Mismatched parentheses");

                    ops.Pop(); // pop '('
                }
                else
                {
                    // Operator
                    while (ops.Count > 0 && Precedence(ops.Peek()) >= Precedence(tok))
                        output.Add(ops.Pop());

                    ops.Push(tok);
                }
            }

            while (ops.Count > 0)
            {
                var op = ops.Pop();
                if (op == "(" || op == ")")
                    throw new Exception("Mismatched parentheses");

                output.Add(op);
            }

            return output;
        }

        private static double EvaluateRpn(List<string> rpn, SpreadsheetState state)
        {
            var stack = new Stack<double>();

            foreach (var tok in rpn)
            {
                // Operator
                if (tok == "+" || tok == "-" || tok == "*" || tok == "/")
                {
                    if (stack.Count < 2)
                        throw new Exception("Invalid expression");

                    var b = stack.Pop();
                    var a = stack.Pop();
                    var res = tok switch
                    {
                        "+" => a + b,
                        "-" => a - b,
                        "*" => a * b,
                        "/" => a / b,
                        _ => throw new Exception($"Unknown operator {tok}")
                    };
                    stack.Push(res);
                }
                // Number literal
                else if (double.TryParse(tok, NumberStyles.Number, CultureInfo.InvariantCulture, out var num))
                {
                    stack.Push(num);
                }
                // Cell reference
                else if (Regex.IsMatch(tok, @"^[A-Za-z0-9_]+\.[A-Za-z0-9_]+$"))
                {
                    var parts = tok.Split('.');
                    var rowName = parts[0];
                    var colName = parts[1];
                    var tbl = state.GetActiveTable();
                    if (tbl == null)
                        throw new Exception("No active table for cell reference");

                    if (!tbl.HasRow(rowName) || !tbl.HasColumn(colName))
                        throw new Exception($"Invalid cell reference '{tok}'");

                    int idx = tbl.Columns.IndexOf(colName);
                    var raw = tbl.Rows[rowName][idx];
                    if (!double.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var val))
                        throw new Exception($"Cell {tok} does not contain a numeric value");

                    stack.Push(val);
                }
                else
                {
                    throw new Exception($"Unrecognized token '{tok}'");
                }
            }

            if (stack.Count != 1)
                throw new Exception("Invalid expression");

            return stack.Pop();
        }
    }
}
