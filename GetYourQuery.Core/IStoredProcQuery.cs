using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Core
{
    public interface IStoredProcQuery
    {
        public void ParamaterNamesGet(DataTable parmsDataTable);

        public string QueryGet(string schemaName, string procedureName, string paramNameAndData);

        public Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName);

        public void ParametersDataGenerate();
    }
}
