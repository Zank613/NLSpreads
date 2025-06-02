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

                // For each existing row, append an empty cell
                foreach (var rowName in Rows.Keys.ToList())
                {
                    Rows[rowName].Add(string.Empty);
                }
            }
        }

        public void DeleteColumn(string columnName)
        {
            int idx = Columns.IndexOf(columnName);
            if (idx >= 0)
            {
                Columns.RemoveAt(idx);

                // Remove that index from every row's value list
                foreach (var rowName in Rows.Keys.ToList())
                {
                    var values = Rows[rowName];
                    if (idx < values.Count)
                    {
                        values.RemoveAt(idx);
                    }
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
            {
                // Initialize with as many empty cells as there are columns
                Rows[rowName] = Columns.Select(c => string.Empty).ToList();
            }
        }

        public void FillRow(string rowName, List<string> values)
        {
            if (Rows.ContainsKey(rowName))
            {
                var newCells = new List<string>();

                // For each column, take the corresponding value or "", if not provided
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (i < values.Count)
                    {
                        newCells.Add(values[i]);
                    }
                    else
                    {
                        newCells.Add(string.Empty);
                    }
                }

                Rows[rowName] = newCells;
            }
        }

        public void DeleteRow(string rowName)
        {
            if (Rows.ContainsKey(rowName))
            {
                Rows.Remove(rowName);
            }
        }
    }
}
