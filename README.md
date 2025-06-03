# NLSpreads
A natural language style CLI spreadsheet made in [Spectre.Console](https://github.com/spectreconsole/spectre.console) and C#.

## Important
> [!WARNING]
> This project is still work in progress.

## Commands
Create and manage simple table data with commands like

- ``create table named "Demo"``
- ``add columns "Name" "Score"``
- ``add rows "Player1" "Player2"``
- ``fill Player1 with "John" "1200"``
- ``show table``

## Features

- Natural language style REPL
- Add/delete rows and columns
- Switch/delete tables

## Roadmap

- [ ] Save and load `.csv`
- [ ] Save and load `.json`
- [ ] Save and load `.xlsx`
- [ ] Arithmetic operations on cells
- [ ] Individual cell editing with `set Row1 Column1 "Data"`
- [ ] More visualization options

## Build

From source run `dotnet run` and it should build.

## License

This project is licensed under MIT License. See `LICENSE` file.