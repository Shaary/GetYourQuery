﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Core
{
    public class DeleteStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        public DeleteStoredProcQuery(DataTable TableNameTable) : base(TableNameTable)
        {
        }
    }
}
