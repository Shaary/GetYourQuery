using System;
using System.Collections.Generic;
using System.Text;

namespace GetYourQuery.Core
{
    public class ColumnTablePair
    {
        public ColumnTablePair(string columnName, string tableName)
        {
            ColumnName = columnName;
            TableName = tableName;
        }

        public string ColumnName { get; }
        public string TableName { get; }
    }
}
