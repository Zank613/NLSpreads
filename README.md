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
- `set "RowName" "ColumnName" to "Value"`

## Features

- Natural language style REPL
- Add/delete rows and columns
- Switch/delete tables
- Edit individual cell data with `set`
- CLI table visualization

## Roadmap

- [ ] Save and load `.csv`
- [ ] Save and load `.json`
- [ ] Save and load `.xlsx`
- [ ] Arithmetic operations on cells
- [x] Individual cell editing with `set Row1 Column1 "Data"`
- [ ] More visualization options

## Build

**.NET 9 required.**

From source run `dotnet run` and it should build.

**OR**

**.NET SDK not required.**

You can use the [Releases](https://github.com/Zank613/NLSpreads/releases) to download compiled binaries.

## License

This project is licensed under [MIT License](./LICENSE).