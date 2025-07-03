using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace NLSpreads
{
    public static class CommandExecutor
    {
        // Help entries for 'help' command
        static readonly Dictionary<string, (string Syntax, string Description)> HelpEntries = new()
        {
            ["show table"]     = ("show table", "Displays the active table."),
            ["create table"]   = ("create table named \"Name\"", "Creates and switches to a new table."),
            ["delete table"]   = ("delete table \"Name\"", "Deletes the named table."),
            ["switch table"]   = ("switch table \"Name\"", "Switches the active table."),
            ["add rows"]       = ("add rows \"R1\" \"R2\"", "Adds rows to the active table."),
            ["delete rows"]    = ("delete rows \"R1\" \"R2\"", "Deletes rows from the active table."),
            ["add columns"]    = ("add columns \"C1\" \"C2\"", "Adds columns to the active table."),
            ["delete columns"] = ("delete columns \"C1\" \"C2\"", "Deletes columns from the active table."),
            ["fill row"]       = ("fill \"Row\" with \"v1\" \"v2\"", "Fills a row with values."),
            ["fill column"]    = ("fill column \"Col\" with \"v1\" \"v2\"", "Fills a column with values."),
            ["set cell"]       = ("set \"Row\" \"Col\" to \"Value\"", "Sets a single cell value."),
            ["export table"]   = ("export table to \"file.csv\"", "Exports the active table to CSV."),
            ["import table"]   = ("import table from \"file.csv\"", "Imports a CSV as a new table."),
            ["rename row"]     = ("rename row \"Old\" to \"New\"", "Renames a row."),
            ["rename column"]  = ("rename column \"Old\" to \"New\"", "Renames a column."),
            ["copy table"]     = ("copy table \"Src\" to \"Dst\"", "Copies a table."),
            ["help"]           = ("help [\"topic\"]", "Shows usage information.")
        };

        public static void Execute(Command command, SpreadsheetState state)
        {
            switch (command)
            {
                case HelpCommand hc:
                    ExecuteHelp(hc);
                    break;
                case ShowTableCommand:
                    ExecuteShow(state);
                    break;
                case CreateTableCommand ctc:
                    ExecuteCreate(ctc, state);
                    break;
                case DeleteTableCommand dtc:
                    ExecuteDeleteTable(dtc, state);
                    break;
                case SwitchTableCommand stc:
                    ExecuteSwitch(stc, state);
                    break;
                case AddRowsCommand arc:
                    ExecuteAddRows(arc, state);
                    break;
                case DeleteRowsCommand drc:
                    ExecuteDeleteRows(drc, state);
                    break;
                case AddColumnsCommand acc:
                    ExecuteAddColumns(acc, state);
                    break;
                case DeleteColumnsCommand dcc:
                    ExecuteDeleteColumns(dcc, state);
                    break;
                case FillRowCommand frc:
                    ExecuteFillRow(frc, state);
                    break;
                case FillColumnCommand fcc:
                    ExecuteFillColumn(fcc, state);
                    break;
                case SetCellCommand scc:
                    ExecuteSetCell(scc, state);
                    break;
                case ExportTableCommand etc:
                    ExecuteExport(etc, state);
                    break;
                case ImportTableCommand itc:
                    ExecuteImport(itc, state);
                    break;
                case RenameRowCommand rrc:
                    ExecuteRenameRow(rrc, state);
                    break;
                case RenameColumnCommand rcc:
                    ExecuteRenameColumn(rcc, state);
                    break;
                case CopyTableCommand ctc2:
                    ExecuteCopy(ctc2, state);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Unhandled command type[/]");
                    break;
            }
        }

        static void ExecuteHelp(HelpCommand cmd)
        {
            if (string.IsNullOrEmpty(cmd.Topic))
            {
                AnsiConsole.MarkupLine("[underline bold]Commands:[/]");
                foreach (var kv in HelpEntries)
                    AnsiConsole.MarkupLine($"[green]{kv.Key}[/] - {kv.Value.Description}");
            }
            else
            {
                var key = HelpEntries.Keys.FirstOrDefault(k => k.Contains(cmd.Topic.ToLowerInvariant()));
                if (key == null)
                    AnsiConsole.MarkupLine($"[red]No help for '{cmd.Topic}'[/]");
                else
                {
                    var entry = HelpEntries[key];
                    AnsiConsole.MarkupLine($"[yellow]Usage:[/] {entry.Syntax}\n[yellow]Description:[/] {entry.Description}");
                }
            }
        }

        static void ExecuteShow(SpreadsheetState state)
        {
            var table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table.[/]");
                return;
            }
            if (table.Rows.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]{table.Name} is empty.[/]");
                return;
            }
            var consoleTable = new Spectre.Console.Table();
            consoleTable.Title($"[bold underline]{table.Name}[/]");
            consoleTable.AddColumn("Row");
            foreach (var col in table.Columns)
                consoleTable.AddColumn(col);
            foreach (var kv in table.Rows)
            {
                var row = new List<string> { kv.Key };
                row.AddRange(kv.Value);
                consoleTable.AddRow(row.ToArray());
            }
            AnsiConsole.Write(consoleTable);
        }

        static void ExecuteCreate(CreateTableCommand cmd, SpreadsheetState st)
        {
            if (string.IsNullOrEmpty(cmd.TableName))
                AnsiConsole.MarkupLine("[red]Table name required.[/]");
            else if (st.HasTable(cmd.TableName))
            {
                st.SwitchTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[yellow]Switched to '{cmd.TableName}'[/]");
            }
            else
            {
                st.CreateTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[green]Created '{cmd.TableName}'[/]");
            }
        }

        static void ExecuteDeleteTable(DeleteTableCommand cmd, SpreadsheetState st)
        {
            if (st.HasTable(cmd.TableName))
            {
                st.DeleteTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[green]Deleted '{cmd.TableName}'[/]");
            }
            else
                AnsiConsole.MarkupLine($"[red]No table '{cmd.TableName}'[/]");
        }

        static void ExecuteSwitch(SwitchTableCommand cmd, SpreadsheetState st)
        {
            if (st.HasTable(cmd.TableName))
            {
                st.SwitchTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[green]Active='{cmd.TableName}'[/]");
            }
            else
                AnsiConsole.MarkupLine($"[red]No table '{cmd.TableName}'[/]");
        }

        static void ExecuteAddRows(AddRowsCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            foreach (var r in cmd.RowNames)
            {
                if (t.HasRow(r))
                    AnsiConsole.MarkupLine($"[yellow]Row '{r}' exists[/]");
                else
                {
                    t.AddRow(r);
                    AnsiConsole.MarkupLine($"[green]Added row '{r}'[/]");
                }
            }
        }

        static void ExecuteDeleteRows(DeleteRowsCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            foreach (var r in cmd.RowNames)
            {
                if (t.HasRow(r))
                {
                    t.DeleteRow(r);
                    AnsiConsole.MarkupLine($"[green]Deleted row '{r}'[/]");
                }
                else
                    AnsiConsole.MarkupLine($"[yellow]No row '{r}'[/]");
            }
        }

        static void ExecuteAddColumns(AddColumnsCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            foreach (var c in cmd.ColumnNames)
            {
                if (t.HasColumn(c))
                    AnsiConsole.MarkupLine($"[yellow]Column '{c}' exists[/]");
                else
                {
                    t.AddColumn(c);
                    AnsiConsole.MarkupLine($"[green]Added column '{c}'[/]");
                }
            }
        }

        static void ExecuteDeleteColumns(DeleteColumnsCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            foreach (var c in cmd.ColumnNames)
            {
                if (t.HasColumn(c))
                {
                    t.DeleteColumn(c);
                    AnsiConsole.MarkupLine($"[green]Deleted column '{c}'[/]");
                }
                else
                    AnsiConsole.MarkupLine($"[yellow]No column '{c}'[/]");
            }
        }

        static void ExecuteFillRow(FillRowCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            if (!t.HasRow(cmd.RowName)) { AnsiConsole.MarkupLine($"[red]No row '{cmd.RowName}'[/]"); return; }
            t.FillRow(cmd.RowName, cmd.Values);
            AnsiConsole.MarkupLine($"[green]Filled row '{cmd.RowName}'[/]");
        }

        static void ExecuteFillColumn(FillColumnCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            if (!t.HasColumn(cmd.ColumnName)) { AnsiConsole.MarkupLine($"[red]No column '{cmd.ColumnName}'[/]"); return; }
            t.FillColumn(cmd.ColumnName, cmd.Values);
            AnsiConsole.MarkupLine($"[green]Filled column '{cmd.ColumnName}'[/]");
        }

        static void ExecuteSetCell(SetCellCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            if (!t.HasRow(cmd.RowName)) { AnsiConsole.MarkupLine($"[red]No row '{cmd.RowName}'[/]"); return; }
            if (!t.HasColumn(cmd.ColumnName)) { AnsiConsole.MarkupLine($"[red]No column '{cmd.ColumnName}'[/]"); return; }

            // formula detection
            if (cmd.Value.StartsWith("="))
            {
                try
                {
                    // strip leading '=' and evaluate
                    var expr = cmd.Value.Substring(1);
                    var result = ExpressionEvaluator.Evaluate(expr, st);
                    t.SetCell(cmd.RowName, cmd.ColumnName, result.ToString(CultureInfo.InvariantCulture));
                    AnsiConsole.MarkupLine($"[green]Computed {cmd.RowName}.{cmd.ColumnName} = {result}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error evaluating formula: {ex.Message}[/]");
                }
            }
            else
            {
                t.SetCell(cmd.RowName, cmd.ColumnName, cmd.Value);
                AnsiConsole.MarkupLine($"[green]Set {cmd.RowName}.{cmd.ColumnName} to '{cmd.Value}'[/]");
            }
        }

        static void ExecuteExport(ExportTableCommand cmd, SpreadsheetState st)
        {
            var t = st.GetActiveTable();
            if (t == null) { AnsiConsole.MarkupLine("[red]No active table.[/]"); return; }
            try
            {
                using var w = new StreamWriter(cmd.FileName);
                string Quote(string s) =>
                    s.Contains(',') || s.Contains('"')
                        ? "\"" + s.Replace("\"", "\"\"") + "\""
                        : s;
                w.WriteLine(string.Join(",", new[] { Quote("Row") }.Concat(t.Columns.Select(Quote))));
                foreach (var kv in t.Rows)
                    w.WriteLine(string.Join(",", new[] { Quote(kv.Key) }.Concat(kv.Value.Select(Quote))));
                AnsiConsole.MarkupLine($"[green]Exported to '{cmd.FileName}'[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Export failed: {ex.Message}[/]");
            }
        }

        static void ExecuteImport(ImportTableCommand cmd, SpreadsheetState st)
        {
            try
            {
                var lines = File.ReadAllLines(cmd.FileName);
                if (lines.Length < 2) throw new Exception("CSV must have header and at least one row.");
                var headers = ParseCsvLine(lines[0]);
                var name = Path.GetFileNameWithoutExtension(cmd.FileName);
                if (st.HasTable(name)) st.DeleteTable(name);
                st.CreateTable(name);
                var tbl = st.GetActiveTable();
                foreach (var col in headers.Skip(1)) tbl.AddColumn(col);
                foreach (var ln in lines.Skip(1))
                {
                    var fields = ParseCsvLine(ln);
                    tbl.AddRow(fields[0]);
                    tbl.FillRow(fields[0], fields.Skip(1).ToList());
                }
                AnsiConsole.MarkupLine($"[green]Imported '{name}'[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Import failed: {ex.Message}[/]");
            }
        }

        static string[] ParseCsvLine(string line)
            => line.Split(',').Select(f => f.Trim(' ', '"')).ToArray();

        static void ExecuteRenameRow(RenameRowCommand cmd, SpreadsheetState st)
        {
            var tbl = st.GetActiveTable();
            if (tbl == null) { AnsiConsole.MarkupLine("[red]No active table.[/]" ); return; }
            if (!tbl.HasRow(cmd.OldName))
                AnsiConsole.MarkupLine($"[red]No row '{cmd.OldName}'[/]");
            else
            {
                var vals = tbl.Rows[cmd.OldName];
                tbl.Rows.Remove(cmd.OldName);
                tbl.Rows[cmd.NewName] = vals;
                AnsiConsole.MarkupLine($"[green]Renamed row '{cmd.OldName}' to '{cmd.NewName}'[/]");
            }
        }

        static void ExecuteRenameColumn(RenameColumnCommand cmd, SpreadsheetState st)
        {
            var tbl = st.GetActiveTable();
            if (tbl == null) { AnsiConsole.MarkupLine("[red]No active table.[/]" ); return; }
            var idx = tbl.Columns.IndexOf(cmd.OldName);
            if (idx < 0)
                AnsiConsole.MarkupLine($"[red]No column '{cmd.OldName}'[/]");
            else
            {
                tbl.Columns[idx] = cmd.NewName;
                AnsiConsole.MarkupLine($"[green]Renamed column '{cmd.OldName}' to '{cmd.NewName}'[/]");
            }
        }

        static void ExecuteCopy(CopyTableCommand cmd, SpreadsheetState st)
        {
            if (!st.HasTable(cmd.Source))
                AnsiConsole.MarkupLine($"[red]No table '{cmd.Source}'[/]");
            else
            {
                var src = st.Tables[cmd.Source];
                var clone = new Table(cmd.Destination);
                foreach (var col in src.Columns) clone.AddColumn(col);
                foreach (var kv in src.Rows) { clone.AddRow(kv.Key); clone.FillRow(kv.Key, new List<string>(kv.Value)); }
                st.Tables[cmd.Destination] = clone;
                st.SwitchTable(cmd.Destination);
                AnsiConsole.MarkupLine($"[green]Copied '{cmd.Source}' to '{cmd.Destination}'[/]");
            }
        }
    }
}