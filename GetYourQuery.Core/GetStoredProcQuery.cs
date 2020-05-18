using System;
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public class GetStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        private List<string> paramNames;
        public GetStoredProcQuery(DataTable TableNameTable, string storedProcName, string schemaName) : base(TableNameTable, storedProcName, schemaName)
        {
            paramNames = new List<string>();
        }

        public override string QueryGet(string schemaName, string paramNameAndData)
        {
            var nonIdParamColumnTable = ParametersDataGet();

            var query = $"exec [{schemaName}].[{storedProcName}] {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")} {nonIdParamColumnTable.Replace(",", Environment.NewLine + ",")}";
            return query;
        }

        public override void ParamaterNamesSet(DataTable parmsDataTable)
        {
            //uses class fileds to get param names like @filter_id_eq

            DataColumn parmName = parmsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in parmsDataTable.Rows)
            {
                if (row[parmName].ToString().Contains("Id"))
                {
                    IdList.Add(row[parmName].ToString());
                }
                else
                {
                    this.paramNames.Add(ColumnNameGet(row[parmName].ToString()));
                }
            }
        }

        private string ParametersDataGet()
        {
            PkId = DataGenerator.PkIdGet(schemaName, tableName, pkName);
            return DataGenerator.DataGet(paramNames, schemaName, tableName, pkName, PkId); 
        }

    }
}
