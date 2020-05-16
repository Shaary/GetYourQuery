
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
    }
}
