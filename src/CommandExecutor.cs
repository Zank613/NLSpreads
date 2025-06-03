using System;
using System.Collections.Generic;
using Spectre.Console;

namespace NLSpreads
{
    public static class CommandExecutor
    {
        public static void Execute(Command command, SpreadsheetState state)
        {
            switch (command)
            {
                case ShowTableCommand stc:
                    ExecuteShowTable(stc, state);
                    break;
                case CreateTableCommand ctc:
                    ExecuteCreateTable(ctc, state);
                    break;
                case DeleteTableCommand dtc:
                    ExecuteDeleteTable(dtc, state);
                    break;
                case SwitchTableCommand stc2:
                    ExecuteSwitchTable(stc2, state);
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
                default:
                    AnsiConsole.MarkupLine("[red]Unknown command type[/]");
                    break;
            }
        }

        private static void ExecuteCreateTable(CreateTableCommand cmd, SpreadsheetState state)
        {
            if (string.IsNullOrEmpty(cmd.TableName))
            {
                AnsiConsole.MarkupLine("[red]Table name cannot be empty.[/]");
                return;
            }
            if (state.HasTable(cmd.TableName))
            {
                state.SwitchTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[yellow]Table '[u]{cmd.TableName}[/]' already exists. Switched to it.[/]");
            }
            else
            {
                state.CreateTable(cmd.TableName);
                AnsiConsole.MarkupLine($"[green]Created table '[u]{cmd.TableName}[/]' and set as active.[/]");
            }
        }

        private static void ExecuteDeleteTable(DeleteTableCommand cmd, SpreadsheetState state)
        {
            if (string.IsNullOrEmpty(cmd.TableName))
            {
                AnsiConsole.MarkupLine("[red]Table name cannot be empty.[/]");
                return;
            }
            if (!state.HasTable(cmd.TableName))
            {
                AnsiConsole.MarkupLine($"[red]Table '[u]{cmd.TableName}[/]' does not exist.[/]");
                return;
            }
            state.DeleteTable(cmd.TableName);
            AnsiConsole.MarkupLine($"[green]Deleted table '[u]{cmd.TableName}[/]'.[/]");
        }

        private static void ExecuteSwitchTable(SwitchTableCommand cmd, SpreadsheetState state)
        {
            if (string.IsNullOrEmpty(cmd.TableName))
            {
                AnsiConsole.MarkupLine("[red]Table name cannot be empty.[/]");
                return;
            }
            if (!state.HasTable(cmd.TableName))
            {
                AnsiConsole.MarkupLine($"[red]Table '[u]{cmd.TableName}[/]' does not exist.[/]");
                return;
            }
            state.SwitchTable(cmd.TableName);
            AnsiConsole.MarkupLine($"[green]Switched to table '[u]{cmd.TableName}[/]'.[/]");
        }

        private static void ExecuteAddRows(AddRowsCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            foreach (string rowName in cmd.RowNames)
            {
                if (table.HasRow(rowName))
                {
                    AnsiConsole.MarkupLine($"[yellow]Row '[u]{rowName}[/]' already exists in table '[u]{table.Name}[/]'.[/]");
                }
                else
                {
                    table.AddRow(rowName);
                    AnsiConsole.MarkupLine($"[green]Added row '[u]{rowName}[/]' to table '[u]{table.Name}[/]'.[/]");
                }
            }
        }

        private static void ExecuteDeleteRows(DeleteRowsCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            foreach (string rowName in cmd.RowNames)
            {
                if (!table.HasRow(rowName))
                {
                    AnsiConsole.MarkupLine($"[yellow]Row '[u]{rowName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                }
                else
                {
                    table.DeleteRow(rowName);
                    AnsiConsole.MarkupLine($"[green]Deleted row '[u]{rowName}[/]' from table '[u]{table.Name}[/]'.[/]");
                }
            }
        }

        private static void ExecuteAddColumns(AddColumnsCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            foreach (string colName in cmd.ColumnNames)
            {
                if (table.HasColumn(colName))
                {
                    AnsiConsole.MarkupLine($"[yellow]Column '[u]{colName}[/]' already exists in table '[u]{table.Name}[/]'.[/]");
                }
                else
                {
                    table.AddColumn(colName);
                    AnsiConsole.MarkupLine($"[green]Added column '[u]{colName}[/]' to table '[u]{table.Name}[/]'.[/]");
                }
            }
        }

        private static void ExecuteDeleteColumns(DeleteColumnsCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            foreach (string colName in cmd.ColumnNames)
            {
                if (!table.HasColumn(colName))
                {
                    AnsiConsole.MarkupLine($"[yellow]Column '[u]{colName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                }
                else
                {
                    table.DeleteColumn(colName);
                    AnsiConsole.MarkupLine($"[green]Deleted column '[u]{colName}[/]' from table '[u]{table.Name}[/]'.[/]");
                }
            }
        }

        private static void ExecuteFillRow(FillRowCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            if (!table.HasRow(cmd.RowName))
            {
                AnsiConsole.MarkupLine($"[red]Row '[u]{cmd.RowName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                return;
            }
            table.FillRow(cmd.RowName, cmd.Values);
            AnsiConsole.MarkupLine($"[green]Filled row '[u]{cmd.RowName}[/]' with values: {string.Join(", ", cmd.Values)}[/]");
        }

        private static void ExecuteFillColumn(FillColumnCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            if (!table.HasColumn(cmd.ColumnName))
            {
                AnsiConsole.MarkupLine($"[red]Column '[u]{cmd.ColumnName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                return;
            }
            table.FillColumn(cmd.ColumnName, cmd.Values);
            AnsiConsole.MarkupLine($"[green]Filled column '[u]{cmd.ColumnName}[/]' with values: {string.Join(", ", cmd.Values)}[/]");
        }

        private static void ExecuteSetCell(SetCellCommand cmd, SpreadsheetState state)
        {
            Table table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table. Create a table first.[/]");
                return;
            }
            if (!table.HasRow(cmd.RowName))
            {
                AnsiConsole.MarkupLine($"[red]Row '[u]{cmd.RowName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                return;
            }
            if (!table.HasColumn(cmd.ColumnName))
            {
                AnsiConsole.MarkupLine($"[red]Column '[u]{cmd.ColumnName}[/]' does not exist in table '[u]{table.Name}[/]'.[/]");
                return;
            }
            table.SetCell(cmd.RowName, cmd.ColumnName, cmd.Value);
            AnsiConsole.MarkupLine($"[green]Set cell '[u]{cmd.RowName}[/].[u]{cmd.ColumnName}[/]' to '{cmd.Value}'[/]");
        }

        private static void ExecuteShowTable(ShowTableCommand cmd, SpreadsheetState state)
        {
            var table = state.GetActiveTable();
            if (table == null)
            {
                AnsiConsole.MarkupLine("[red]No active table to show. Create one first.[/]");
                return;
            }
            if (table.Rows.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]Table '[u]{table.Name}[/]' is empty (no rows).[/]");
                return;
            }
            var ansiTable = new Spectre.Console.Table();
            ansiTable.Title($"[green bold underline]{table.Name}[/]");
            ansiTable.AddColumn("[green bold][u]Row[/][/]");
            foreach (var colName in table.Columns)
                ansiTable.AddColumn(colName);
            foreach (var kvp in table.Rows)
            {
                string rowName = kvp.Key;
                var values = kvp.Value;
                var cells = new List<string> { rowName };
                cells.AddRange(values);
                while (cells.Count < table.Columns.Count + 1)
                    cells.Add(string.Empty);
                ansiTable.AddRow(cells.ToArray());
            }
            AnsiConsole.Write(ansiTable);
        }
    }
}
