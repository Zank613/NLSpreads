using System.Collections.Generic;

namespace NLSpreads
{
    public class SpreadsheetState
    {
        // All tables by name
        public Dictionary<string, Table> Tables { get; }

        // The name of the currently active table (or null if none)
        public string ActiveTableName { get; private set; }

        public SpreadsheetState()
        {
            Tables = new Dictionary<string, Table>();
            ActiveTableName = null;
        }

        public bool HasTable(string tableName)
        {
            return Tables.ContainsKey(tableName);
        }

        // Returns the active Table object (or null if none)
        public Table GetActiveTable()
        {
            if (ActiveTableName != null
                && Tables.ContainsKey(ActiveTableName))
            {
                return Tables[ActiveTableName];
            }

            return null;
        }

        // Create a new table and set it as active
        public void CreateTable(string tableName)
        {
            if (!Tables.ContainsKey(tableName))
            {
                Tables[tableName] = new Table(tableName);
                ActiveTableName = tableName;
            }
        }

        // Switch to an existing table
        public void SwitchTable(string tableName)
        {
            if (Tables.ContainsKey(tableName))
            {
                ActiveTableName = tableName;
            }
        }

        // Delete a table; if it was active, clear the active reference
        public void DeleteTable(string tableName)
        {
            if (Tables.ContainsKey(tableName))
            {
                Tables.Remove(tableName);

                if (ActiveTableName == tableName)
                {
                    ActiveTableName = null;
                }
            }
        }
    }
}