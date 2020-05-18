﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Core
{
    public class UpdateStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        public UpdateStoredProcQuery(DataTable TableNameTable, string storedProcName, string schemaName) : base(TableNameTable, storedProcName, schemaName)
        {
        }
    }
}
