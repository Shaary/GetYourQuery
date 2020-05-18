using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public class AddStoredProcQuery : StoredProcQuery
    {
        private List<string> excludeParameres = new List<string> { "@DeletedByUserId" };

        public AddStoredProcQuery(DataTable TableNameTable, string storedProcName, string schemaName) : base(TableNameTable, storedProcName, schemaName)
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
