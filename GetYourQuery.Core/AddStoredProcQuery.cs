using System;
using System.Collections.Generic;
using System.Text;

namespace GetYourQuery.Core
{
    class AddStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        public override Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

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

                        if (IsTableExists(tableName))
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
