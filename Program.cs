using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spectre.Console;

namespace NLSpreads
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new SpreadsheetState();

            AnsiConsole.MarkupLine(
                "[gold1 bold]Welcome to NLSpread REPL![/] [green]Type 'exit' to quit or 'help' for commands[/]\n");

            while (true)
            {
                Console.Write("> ");
                string input = ReadInputWithAutocomplete();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine("[gold1 bold]Goodbye![/]");
                    break;
                }

                var cmd = CommandParser.Parse(input);
                if (cmd == null)
                {
                    AnsiConsole.MarkupLine("[red]Unknown command. Type 'help' to see available commands.[/]");
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
                    if (buffer.Length == 0) continue;
                    var partial = buffer.ToString();
                    var suggestions = GetSuggestions(partial);
                    if (suggestions.Count == 0) continue;
                    var selection = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Suggestions:")
                            .AddChoices(suggestions)
                    );
                    int oldLen = buffer.Length;
                    buffer.Clear().Append(selection);
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
            var all = new[]
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
                "set ",
                "export table to ",
                "import table from ",
                "rename row ",
                "rename column ",
                "copy table ",
                "help"
            };
            var list = new List<string>();
            foreach (var cmd in all)
                if (cmd.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                    list.Add(cmd);
            return list;
        }
    }
}