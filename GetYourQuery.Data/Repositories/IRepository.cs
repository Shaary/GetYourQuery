
using GetYourQuery.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Data
{
    public interface IRepository
    {
        public List<string> SchemaList { get; set; }
        DataTable ParametersTableGet(string procedure, string schema);
        string IdParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable);
        List<string> StoredProcedureNamesGet(string schema, string database, string procType);
        DataTable TableNamesGet();
        List<string> SchemaNamesGet();
        string RelatedParametersDataGet();
        string NonIdParametersDataGet(Dictionary<string, string> NonIdDict, string schema, string tableName, string pkName, string pk);
        string PrimaryKeyGet(string schema, string tableName, string pkName);
        string DateLastUpdatedGet(string schema, string tableName, string pkName, string pk);
        string ParametersDataGet(Dictionary<string, string> NonIdDict);
        public bool IsTableExists(string tableName, string schema);
        public Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schema);
        string DataGet(string text, string schema, string procType);
    }
}
