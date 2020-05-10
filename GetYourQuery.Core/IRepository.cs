using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public interface IRepository
    {
        DataTable ParametersTableGet();
        string ParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable);
    }
}
