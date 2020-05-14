
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public interface IRepository
    {
        DataTable ParametersTableGet(string procedure, string schema);
        string ParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable);

        DataTable StoredProcedureNamesGet(string schema);
        DataTable TableNamesGet(string schema);
        List<string> SchemaNamesGet();
    }
}
