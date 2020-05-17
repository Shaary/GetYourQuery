using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    //TODO: Create list of ignored parameter. Check if name in it. like var matchingvalues = myList.Where(stringToCheck => stringToCheck.Contains(myString));
    public class AddStoredProcQuery : StoredProcQuery
    {
        private List<string> excludeParameres = new List<string> { "@DeletedByUserId" };

        public AddStoredProcQuery(DataTable TableNameTable, string storedProcName) : base(TableNameTable, storedProcName)
        {
        }

        public override Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            var insertTableName = storedProcName.Replace("usp_", "")
                                                .Replace("Add", "");

            foreach (var name in IdList)
            {
                if (!excludeParameres.Contains(name))
                {
                    if (name.Contains("ByUser"))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));
                    }
                    else
                    {
                        var tableName = TableNameGet(name);
                        var columnName = ColumnNameGet(name);

                        if (IsTableExists(tableName) && tableName != insertTableName)
                        {
                            paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schemaName}].[{tableName}]"));
                        }
                    }
                };
            }

            return paramColumnTable;
        }

    }
}
