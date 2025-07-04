using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NLSpreads
{
    /// <summary>
    /// Parses and evaluates arithmetic expressions with cell references (e.g. "A1.B2"), math functions (sin, cos, sqrt, etc.),
    /// and named constants (PI, E, or user-defined) using the Shunting-Yard algorithm and RPN evaluation.
    /// Supports +, -, *, /, parentheses, commas (for function args), functions, and constants.
    /// </summary>
    public static class ExpressionEvaluator
    {
        // Whitelisted math functions (unary)
        private static readonly HashSet<string> Functions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "sin","cos","tan","asin","acos","atan",
            "sqrt","log","log10","exp","abs","floor","ceil"
        };

        // Named constants (case-insensitive)
        private static readonly Dictionary<string,double> Constants = new Dictionary<string,double>(StringComparer.OrdinalIgnoreCase)
        {
            { "PI", Math.PI },
            { "E", Math.E }
        };

        /// <summary>
        /// Adds or updates a named constant usable in expressions.
        /// </summary>
        public static void AddConstant(string name, double value)
        {
            if (string.IsNullOrWhiteSpace(name) || !Regex.IsMatch(name, "^[A-Za-z_][A-Za-z0-9_]*$"))
                throw new ArgumentException("Invalid constant name", nameof(name));
            Constants[name] = value;
        }

        /// <summary>
        /// Evaluates the given infix expression string against the provided spreadsheet state.
        /// </summary>
        public static double Evaluate(string expression, SpreadsheetState state)
        {
            var tokens = Tokenize(expression);
            var rpn = ToRpn(tokens);
            return EvaluateRpn(rpn, state);
        }

        // Tokenizes numbers, cell refs (Row.Col), identifiers (funcs/constants), operators, parentheses, and commas
        private static List<string> Tokenize(string expr)
        {
            var pattern = @"([A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*)|([A-Za-z_][A-Za-z0-9_]*)|(\d+(\.\d+)?)|[\+\-\*\/\(\),]";
            var matches = Regex.Matches(expr, pattern);
            var list = new List<string>(matches.Count);
            foreach (Match m in matches)
                list.Add(m.Value);
            return list;
        }

        // Converts tokens to Reverse Polish Notation using the Shunting-Yard algorithm
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
                if (double.TryParse(tok, NumberStyles.Number, CultureInfo.InvariantCulture, out _) ||
                    Regex.IsMatch(tok, @"^[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*$") ||
                    Constants.ContainsKey(tok))
                {
                    // number, cell reference, or constant
                    output.Add(tok);
                }
                else if (Functions.Contains(tok))
                {
                    // function name
                    ops.Push(tok);
                }
                else if (tok == ",")
                {
                    // function argument separator
                    while (ops.Count > 0 && ops.Peek() != "(")
                        output.Add(ops.Pop());
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

                    // if top of stack is a function, pop it to output
                    if (ops.Count > 0 && Functions.Contains(ops.Peek()))
                        output.Add(ops.Pop());
                }
                else
                {
                    // operator
                    while (ops.Count > 0 && Precedence(ops.Peek()) >= Precedence(tok))
                        output.Add(ops.Pop());
                    ops.Push(tok);
                }
            }

            // drain remaining operators
            while (ops.Count > 0)
            {
                var op = ops.Pop();
                if (op == "(" || op == ")")
                    throw new Exception("Mismatched parentheses");
                output.Add(op);
            }

            return output;
        }

        // Evaluates the RPN token list
        private static double EvaluateRpn(List<string> rpn, SpreadsheetState state)
        {
            var stack = new Stack<double>();

            foreach (var tok in rpn)
            {
                // binary operators
                if (tok is "+" or "-" or "*" or "/")
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
                // numeric literal
                else if (double.TryParse(tok, NumberStyles.Number, CultureInfo.InvariantCulture, out var num))
                {
                    stack.Push(num);
                }
                // cell reference
                else if (Regex.IsMatch(tok, @"^[A-Za-z_][A-Za-z0-9_]*\.[A-Za-z_][A-Za-z0-9_]*$"))
                {
                    var parts = tok.Split('.');
                    var row = parts[0];
                    var col = parts[1];
                    var tbl = state.GetActiveTable();
                    if (tbl == null)
                        throw new Exception("No active table for cell reference");

                    if (!tbl.HasRow(row) || !tbl.HasColumn(col))
                        throw new Exception($"Invalid cell reference '{tok}'");

                    int idx = tbl.Columns.IndexOf(col);
                    var raw = tbl.Rows[row][idx];
                    if (!double.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out var val))
                        throw new Exception($"Cell {tok} does not contain a numeric value");

                    stack.Push(val);
                }
                // function call (unary)
                else if (Functions.Contains(tok))
                {
                    if (stack.Count < 1)
                        throw new Exception("Invalid function invocation");

                    var a = stack.Pop();
                    var res = tok.ToLowerInvariant() switch
                    {
                        "sin" => Math.Sin(a),
                        "cos" => Math.Cos(a),
                        "tan" => Math.Tan(a),
                        "asin" => Math.Asin(a),
                        "acos" => Math.Acos(a),
                        "atan" => Math.Atan(a),
                        "sqrt" => Math.Sqrt(a),
                        "log" => Math.Log(a),
                        "log10" => Math.Log10(a),
                        "exp" => Math.Exp(a),
                        "abs" => Math.Abs(a),
                        "floor" => Math.Floor(a),
                        "ceil" => Math.Ceiling(a),
                        _ => throw new Exception($"Unknown function '{tok}'")
                    };
                    stack.Push(res);
                }
                // named constant
                else if (Constants.ContainsKey(tok))
                {
                    stack.Push(Constants[tok]);
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
