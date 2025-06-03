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

        public bool HasColumn(string columnName)
        {
            return Columns.Contains(columnName);
        }

        public void AddColumn(string columnName)
        {
            if (!Columns.Contains(columnName))
            {
                Columns.Add(columnName);
                foreach (var rowName in Rows.Keys.ToList())
                    Rows[rowName].Add(string.Empty);
            }
        }

        public void DeleteColumn(string columnName)
        {
            int idx = Columns.IndexOf(columnName);
            if (idx >= 0)
            {
                Columns.RemoveAt(idx);
                foreach (var rowName in Rows.Keys.ToList())
                {
                    var values = Rows[rowName];
                    if (idx < values.Count)
                        values.RemoveAt(idx);
                }
            }
        }

        public bool HasRow(string rowName)
        {
            return Rows.ContainsKey(rowName);
        }

        public void AddRow(string rowName)
        {
            if (!Rows.ContainsKey(rowName))
                Rows[rowName] = Columns.Select(c => string.Empty).ToList();
        }

        public void FillRow(string rowName, List<string> values)
        {
            if (Rows.ContainsKey(rowName))
            {
                var newCells = new List<string>();
                for (int i = 0; i < Columns.Count; i++)
                {
                    newCells.Add(i < values.Count ? values[i] : string.Empty);
                }
                Rows[rowName] = newCells;
            }
        }

        public void DeleteRow(string rowName)
        {
            if (Rows.ContainsKey(rowName))
                Rows.Remove(rowName);
        }

        public void FillColumn(string columnName, List<string> values)
        {
            int idx = Columns.IndexOf(columnName);
            if (idx < 0) return;
            var rowNames = Rows.Keys.ToList();
            for (int i = 0; i < rowNames.Count; i++)
            {
                var rowName = rowNames[i];
                var rowValues = Rows[rowName];
                string val = i < values.Count ? values[i] : string.Empty;
                if (idx < rowValues.Count)
                    rowValues[idx] = val;
                else
                {
                    while (rowValues.Count < Columns.Count)
                        rowValues.Add(string.Empty);
                    rowValues[idx] = val;
                }
            }
        }

        public void SetCell(string rowName, string columnName, string value)
        {
            int colIdx = Columns.IndexOf(columnName);
            if (colIdx < 0) return;
            if (!Rows.ContainsKey(rowName)) return;
            var rowValues = Rows[rowName];
            while (rowValues.Count < Columns.Count)
                rowValues.Add(string.Empty);
            rowValues[colIdx] = value;
        }
    }
}