using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NLSpreads
{
    public static class CommandParser
    {
        public static Command Parse(string input)
        {
            var tokens = Tokenize(input);
            if (tokens == null || tokens.Count == 0)
            {
                return null;
            }

            var lowerTokens = tokens.Select(t => t.ToLowerInvariant()).ToList();

            // show table
            if (lowerTokens.Count == 2
                && lowerTokens[0] == "show"
                && lowerTokens[1] == "table")
            {
                return new ShowTableCommand();
            }

            // create table named <Name>
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

            // delete table <Name>
            if (lowerTokens.Count == 3
                && lowerTokens[0] == "delete"
                && lowerTokens[1] == "table")
            {
                return new DeleteTableCommand
                {
                    TableName = tokens[2]
                };
            }

            // switch table <Name>
            if (lowerTokens.Count == 3
                && lowerTokens[0] == "switch"
                && lowerTokens[1] == "table")
            {
                return new SwitchTableCommand
                {
                    TableName = tokens[2]
                };
            }

            // add rows <Row1> <Row2> ...
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

            // delete rows <Row1> <Row2> ...
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

            // add columns <Col1> <Col2> ...
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

            // delete columns <Col1> <Col2> ...
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

            // fill column <ColName> with <Val1> <Val2> ...
            if (lowerTokens.Count >= 5
                && lowerTokens[0] == "fill"
                && lowerTokens[1] == "column"
                && lowerTokens[3] == "with")
            {
                string colName = tokens[2];
                var values = tokens.Skip(4).ToList();
                return new FillColumnCommand
                {
                    ColumnName = colName,
                    Values = values
                };
            }

            // fill <RowName> with <Val1> <Val2> ...
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

            return null;
        }

        private static List<string> Tokenize(string input)
        {
            var matches = Regex.Matches(input, @"""([^""]*)""|(\S+)");
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