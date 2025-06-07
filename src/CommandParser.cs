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
            if (tokens.Count == 0) return null;
            var lower = tokens.Select(t => t.ToLowerInvariant()).ToList();

            if (lower[0] == "help")
                return new HelpCommand { Topic = tokens.Count > 1 ? tokens[1] : null };

            if (lower.SequenceEqual(new[] { "show", "table" }))
                return new ShowTableCommand();

            if (lower.Count == 4 && lower[0] == "create" && lower[1] == "table" && lower[2] == "named")
                return new CreateTableCommand { TableName = tokens[3] };

            if (lower.Count == 3 && lower[0] == "delete" && lower[1] == "table")
                return new DeleteTableCommand { TableName = tokens[2] };

            if (lower.Count == 3 && lower[0] == "switch" && lower[1] == "table")
                return new SwitchTableCommand { TableName = tokens[2] };

            if (lower.Count >= 3 && lower[0] == "add" && lower[1] == "rows")
                return new AddRowsCommand { RowNames = tokens.Skip(2).ToList() };

            if (lower.Count >= 3 && lower[0] == "delete" && lower[1] == "rows")
                return new DeleteRowsCommand { RowNames = tokens.Skip(2).ToList() };

            if (lower.Count >= 3 && lower[0] == "add" && lower[1] == "columns")
                return new AddColumnsCommand { ColumnNames = tokens.Skip(2).ToList() };

            if (lower.Count >= 3 && lower[0] == "delete" && lower[1] == "columns")
                return new DeleteColumnsCommand { ColumnNames = tokens.Skip(2).ToList() };

            if (lower.Count >= 4 && lower[0] == "fill" && lower[2] == "with")
                return new FillRowCommand { RowName = tokens[1], Values = tokens.Skip(3).ToList() };

            if (lower.Count >= 5 && lower[0] == "fill" && lower[1] == "column" && lower[3] == "with")
                return new FillColumnCommand { ColumnName = tokens[2], Values = tokens.Skip(4).ToList() };

            if (lower.Count == 5 && lower[0] == "set" && lower[3] == "to")
                return new SetCellCommand { RowName = tokens[1], ColumnName = tokens[2], Value = tokens[4] };

            if (lower.Count == 4 && lower[0] == "export" && lower[1] == "table" && lower[2] == "to")
                return new ExportTableCommand { FileName = tokens[3] };

            if (lower.Count == 4 && lower[0] == "import" && lower[1] == "table" && lower[2] == "from")
                return new ImportTableCommand { FileName = tokens[3] };

            if (lower.Count == 5 && lower[0] == "rename" && lower[1] == "row" && lower[3] == "to")
                return new RenameRowCommand { OldName = tokens[2], NewName = tokens[4] };

            if (lower.Count == 5 && lower[0] == "rename" && lower[1] == "column" && lower[3] == "to")
                return new RenameColumnCommand { OldName = tokens[2], NewName = tokens[4] };

            if (lower.Count == 5 && lower[0] == "copy" && lower[1] == "table" && lower[3] == "to")
                return new CopyTableCommand { Source = tokens[2], Destination = tokens[4] };

            return null;
        }

        static List<string> Tokenize(string input)
        {
            var matches = Regex.Matches(input, @"""([^""]*)""|(\S+)");
            var list = new List<string>();
            foreach (Match m in matches)
                list.Add(m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value);
            return list;
        }
    }
}
