using System;
using Spectre.Console;

namespace NLSpreads
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new SpreadsheetState();
            
            AnsiConsole.MarkupLine(
                "[gold1 bold]Welcome to NLSpread REPL![/] [green]Type 'exit' to quit[/]"
            );

            while (true)
            {
                string input = AnsiConsole.Prompt(
                    new TextPrompt<string>("[blue]> [/]")
                );

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
                    AnsiConsole.MarkupLine("    [green1 bold]fill Row1 with \"foo\" \"bar\"[/]");
                    AnsiConsole.MarkupLine("    [green1 bold]show table[/]");
                    continue;
                }
                
                CommandExecutor.Execute(cmd, state);
            }
        }
    }
}
