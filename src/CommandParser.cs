using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NLSpreads
{
    public static class CommandParser
    {
        /// <summary>
        /// Main entry point: tries to parse input as a known command.
        /// </summary>
        public static Command Parse(string input)
        {
            var tokens = Tokenize(input);

            if (tokens == null || tokens.Count == 0)
            {
                return null;
            }

            // Lowercase all tokens for keyword matching, but keep originals in 'tokens'
            var lowerTokens = tokens.Select(t => t.ToLowerInvariant()).ToList();

            // 0) show table
            if (lowerTokens.Count == 2
                && lowerTokens[0] == "show"
                && lowerTokens[1] == "table")
            {
                return new ShowTableCommand();
            }

            // 1) create table named <Name>
            //    expects exactly 4 tokens: "create", "table", "named", "<Name>"
            if (lowerTokens.Count == 4
                && lowerTokens[0] == "create"
                && lowerTokens[1] == "table"
                && lowerTokens[2] == "named")
            {
                return new CreateTableCommand
                {
                    TableName = tokens[3]
                };
            }

            // 2) delete table <Name>
            if (lowerTokens.Count == 3
                && lowerTokens[0] == "delete"
                && lowerTokens[1] == "table")
            {
                return new DeleteTableCommand
                {
                    TableName = tokens[2]
                };
            }

            // 3) switch table <Name>
            if (lowerTokens.Count == 3
                && lowerTokens[0] == "switch"
                && lowerTokens[1] == "table")
            {
                return new SwitchTableCommand
                {
                    TableName = tokens[2]
                };
            }

            // 4) add rows <Row1> <Row2> ...
            if (lowerTokens.Count >= 3
                && lowerTokens[0] == "add"
                && lowerTokens[1] == "rows")
            {
                var rowNames = tokens.Skip(2).ToList();
                return new AddRowsCommand
                {
                    RowNames = rowNames
                };
            }

            // 5) delete rows <Row1> <Row2> ...
            if (lowerTokens.Count >= 3
                && lowerTokens[0] == "delete"
                && lowerTokens[1] == "rows")
            {
                var rowNames = tokens.Skip(2).ToList();
                return new DeleteRowsCommand
                {
                    RowNames = rowNames
                };
            }

            // 6) add columns <Col1> <Col2> ...
            if (lowerTokens.Count >= 3
                && lowerTokens[0] == "add"
                && lowerTokens[1] == "columns")
            {
                var columnNames = tokens.Skip(2).ToList();
                return new AddColumnsCommand
                {
                    ColumnNames = columnNames
                };
            }

            // 7) delete columns <Col1> <Col2> ...
            if (lowerTokens.Count >= 3
                && lowerTokens[0] == "delete"
                && lowerTokens[1] == "columns")
            {
                var columnNames = tokens.Skip(2).ToList();
                return new DeleteColumnsCommand
                {
                    ColumnNames = columnNames
                };
            }

            // 8) fill <RowName> with <Val1> <Val2> ...
            if (lowerTokens.Count >= 4
                && lowerTokens[0] == "fill"
                && lowerTokens[2] == "with")
            {
                string rowName = tokens[1];
                var values = tokens.Skip(3).ToList();
                return new FillRowCommand
                {
                    RowName = rowName,
                    Values = values
                };
            }

            // If nothing matched:
            return null;
        }

        /// <summary>
        /// Splits the input string into tokens, treating quoted text as a single token.
        /// e.g. create table named "My table" -> ["create","table","named","My table"]
        /// </summary>
        private static List<string> Tokenize(string input)
        {
            // Regex: match either "quoted text" OR a sequence of non-space characters
            var matches = Regex.Matches(input, @"\""([^\""]*)\""|(\S+)");
            var tokens = new List<string>();

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    tokens.Add(match.Groups[1].Value);
                }
                else
                {
                    tokens.Add(match.Groups[2].Value);
                }
            }

            return tokens;
        }
    }
}