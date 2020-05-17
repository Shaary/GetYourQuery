using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Core
{
    public interface IStoredProcQuery
    {
        public void ParamaterNamesSet(DataTable parmsDataTable);

        public string QueryGet(string schemaName, string paramNameAndData);

        public Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName);

        //public void ParametersDataGenerate();

        //public bool IsNameExists(string procName);

        public bool IsTableExists(string tableName);
    }
}
