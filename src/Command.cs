using System.Collections.Generic;

namespace NLSpreads
{
    public abstract class Command { }

    public class ShowTableCommand : Command { }
    public class CreateTableCommand : Command { public string TableName { get; set; } }
    public class DeleteTableCommand : Command { public string TableName { get; set; } }
    public class SwitchTableCommand : Command { public string TableName { get; set; } }

    public class AddRowsCommand : Command { public List<string> RowNames { get; set; } }
    public class DeleteRowsCommand : Command { public List<string> RowNames { get; set; } }

    public class AddColumnsCommand : Command { public List<string> ColumnNames { get; set; } }
    public class DeleteColumnsCommand : Command { public List<string> ColumnNames { get; set; } }

    public class FillRowCommand : Command { public string RowName { get; set; } public List<string> Values { get; set; } }
    public class FillColumnCommand : Command { public string ColumnName { get; set; } public List<string> Values { get; set; } }
    public class SetCellCommand : Command { public string RowName { get; set; } public string ColumnName { get; set; } public string Value { get; set; } }

    public class ExportTableCommand : Command { public string FileName { get; set; } }
    public class ImportTableCommand : Command { public string FileName { get; set; } }

    public class RenameRowCommand : Command { public string OldName { get; set; } public string NewName { get; set; } }
    public class RenameColumnCommand : Command { public string OldName { get; set; } public string NewName { get; set; } }
    public class CopyTableCommand : Command { public string Source { get; set; } public string Destination { get; set; } }
    public class HelpCommand : Command { public string Topic { get; set; } }
}