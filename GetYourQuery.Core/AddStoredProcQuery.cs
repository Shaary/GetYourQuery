using System;
using System.Collections.Generic;
using System.Text;

namespace GetYourQuery.Core
{
    class AddStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        public Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName, string storedProcName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            var insertTableName = storedProcName.Replace("usp_", "")
                                                .Replace("Add", "");

            foreach (var name in IdList)
            {
                if (name.Contains("Id"))
                {
                    if (name.Contains("ByUser"))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));
                    }
                    else
                    {
                        var tableName = name.Replace("@filter_", "")
                                .Replace("_eq", "")
                                .Replace("@", "")
                                .Replace("Id", "") + "s";
                        var columnName = (name.Replace("@filter_", "")
                                        .Replace("_eq", "")
                                        .Replace("@", "")
                                        );

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
