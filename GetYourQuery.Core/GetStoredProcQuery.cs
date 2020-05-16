using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Core
{
    public class GetStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        public GetStoredProcQuery(DataTable TableNameTable) : base(TableNameTable)
        {
        }
    }
}
