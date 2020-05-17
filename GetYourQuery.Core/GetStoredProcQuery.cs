using System;
using System.Data;
using System.Net.WebSockets;

namespace GetYourQuery.Core
{
    public class GetStoredProcQuery : StoredProcQuery, IStoredProcQuery
    {
        private string paramNames = "";
        public GetStoredProcQuery(DataTable TableNameTable, string storedProcName) : base(TableNameTable, storedProcName)
        {
        }

        public override string QueryGet(string schemaName, string paramNameAndData)
        {
            var pkName = storedProcName.Replace("usp_", "")
                                       .Replace("sGet", "") + "Id";

            var nonIdParamColumnTable = ParametersDataGenerate();

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
                //For non-id parameters I need to know data type to generate values for add and update
                else
                {
                    this.paramNames += row[parmName].ToString();
                }
            }
        }

    }
}
