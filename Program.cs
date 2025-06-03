using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

namespace NLSpreads
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new SpreadsheetState();

            AnsiConsole.MarkupLine("[gold1 bold]Welcome to NLSpread REPL![/] [green]Type 'exit' to quit[/]");

            while (true)
            {
                Console.Write("> ");
                string input = ReadInputWithAutocomplete();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine("[gold1 bold]Goodbye![/]");
                    break;
                }

                Command cmd = CommandParser.Parse(input);

                if (cmd == null)
                {
                    AnsiConsole.MarkupLine("[red bold]I didn't understand that.[/]");
                    AnsiConsole.WriteLine("Try:");
                    AnsiConsole.MarkupLine("    [green1 bold]create table named \"My table\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]delete table \"My table\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]switch table \"My table\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]add rows \"Row1\" \"Row2\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]delete rows \"Row1\" \"Row2\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]add columns \"Col1\" \"Col2\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]delete columns \"Col1\" \"Col2\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]fill \"Row1\" with \"foo\" \"bar\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]fill column \"Col1\" with \"foo\" \"bar\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]set \"RowName\" \"ColName\" to \"Value\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]show table[/]");
                    continue;
                }

                CommandExecutor.Execute(cmd, state);
            }
        }

        static string ReadInputWithAutocomplete()
        {
            var buffer = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Remove(buffer.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if (key.Key == ConsoleKey.Tab)
                {
                    if (buffer.Length == 0)
                        continue;
                    var partial = buffer.ToString();
                    var suggestions = GetSuggestions(partial);
                    if (suggestions.Count == 0)
                        continue;
                    var selection = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Suggestions:")
                            .AddChoices(suggestions)
                    );
                    int oldLen = buffer.Length;
                    buffer.Clear();
                    buffer.Append(selection);
                    Console.Write("\r> " + selection + new string(' ', Math.Max(0, oldLen - selection.Length)));
                    Console.Write("\r> " + selection);
                }
                else
                {
                    buffer.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
            }
            return buffer.ToString();
        }

        static List<string> GetSuggestions(string input)
        {
            var allCommands = new List<string>
            {
                "show table",
                "create table named ",
                "delete table ",
                "switch table ",
                "add rows ",
                "delete rows ",
                "add columns ",
                "delete columns ",
                "fill ",
                "fill column ",
                "set "
            };
            var matches = new List<string>();
            foreach (var cmd in allCommands)
            {
                if (cmd.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(cmd);
                }
            }
            return matches;
        }
    }
}