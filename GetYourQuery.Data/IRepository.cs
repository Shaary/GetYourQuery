
using System;
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public interface IRepository
    {
        DataTable ParametersTableGet(string procedure, string schema);
        string IdParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable);

        List<string> StoredProcedureNamesGet(string schema, string database, string procType);
        DataTable TableNamesGet(string schema);
        List<string> SchemaNamesGet();

        string RelatedParametersDataGet();
        string NonIdParametersDataGet(List<string> paramNames, string schema, string tableName, string pkName, string pk);
        string PrimaryKeyGet(string schema, string tableName, string pkName);
        string DateLastUpdatedGet(string schema, string tableName, string pkName, string pk);
    }
}
