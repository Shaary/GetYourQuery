using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GetYourQuery.Core
{
    public class StoredProcQuery 
    {

        public static string QueryGet(string schemaName, string storedProcName, string paramNameAndData)
        {
            var query = $"exec [{schemaName}].[{storedProcName}] {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")}";
            return query;
        }

    }
}
