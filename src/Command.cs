using System.Collections.Generic;

namespace NLSpreads
{
    // base class for any parsed command
    public abstract class Command
    {
        
    }
    
    // create table named Testing
    public class CreateTableCommand : Command
    {
        public string TableName { get; set; }
    }
    
    // Add rows "Test1" "Test2" etc.
    public class AddRowsCommand : Command
    {
        public List<string> RowNames { get; set; }
    }
    
    // fill Test1 with "val1" val2"
    public class FillRowCommand : Command
    {
        public string RowName { get; set; }
        public List<string> Values { get; set; }
    }

    public class ShowTableCommand : Command
    {
        
    }

    public class AddColumnsCommand : Command
    {
        public List<string> ColumnNames { get; set; }
    }

    public class DeleteTableCommand : Command
    {
        public string TableName { get; set; }
    }

    public class SwitchTableCommand : Command
    {
        public string TableName { get; set; }
    }

    public class DeleteRowsCommand : Command
    {
        public List<string> RowNames { get; set; }
    }

    public class DeleteColumnsCommand : Command
    {
        public List<string> ColumnNames { get; set; }
    }
}