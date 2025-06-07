using System.Collections.Generic;
using System.Linq;

namespace NLSpreads
{
    public class Table
    {
        public string Name { get; }
        public List<string> Columns { get; }
        public Dictionary<string, List<string>> Rows { get; }

        public Table(string name)
        {
            Name = name;
            Columns = new List<string>();
            Rows = new Dictionary<string, List<string>>();
        }

        public bool HasColumn(string columnName) => Columns.Contains(columnName);

        public void AddColumn(string columnName)
        {
            if (!Columns.Contains(columnName))
            {
                Columns.Add(columnName);
                foreach (var row in Rows.Keys.ToList())
                    Rows[row].Add(string.Empty);
            }
        }

        public void DeleteColumn(string columnName)
        {
            var idx = Columns.IndexOf(columnName);
            if (idx < 0) return;
            Columns.RemoveAt(idx);
            foreach (var row in Rows.Keys.ToList())
            {
                var vals = Rows[row];
                if (idx < vals.Count) vals.RemoveAt(idx);
            }
        }

        public bool HasRow(string rowName) => Rows.ContainsKey(rowName);

        public void AddRow(string rowName)
        {
            if (!Rows.ContainsKey(rowName))
                Rows[rowName] = Columns.Select(_ => string.Empty).ToList();
        }

        public void FillRow(string rowName, List<string> values)
        {
            if (!Rows.ContainsKey(rowName)) return;
            var newVals = new List<string>();
            for (int i = 0; i < Columns.Count; i++)
                newVals.Add(i < values.Count ? values[i] : string.Empty);
            Rows[rowName] = newVals;
        }

        public void DeleteRow(string rowName)
        {
            if (Rows.ContainsKey(rowName)) Rows.Remove(rowName);
        }

        public void FillColumn(string columnName, List<string> values)
        {
            var idx = Columns.IndexOf(columnName);
            if (idx < 0) return;
            var keys = Rows.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var row = Rows[keys[i]];
                var val = i < values.Count ? values[i] : string.Empty;
                if (idx < row.Count) row[idx] = val;
                else
                {
                    while (row.Count <= idx) row.Add(string.Empty);
                    row[idx] = val;
                }
            }
        }

        public void SetCell(string rowName, string columnName, string value)
        {
            var idx = Columns.IndexOf(columnName);
            if (idx < 0 || !Rows.ContainsKey(rowName)) return;
            var row = Rows[rowName];
            while (row.Count < Columns.Count) row.Add(string.Empty);
            row[idx] = value;
        }
    }
}
