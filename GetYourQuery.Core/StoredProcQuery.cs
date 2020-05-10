using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Core
{
    public class StoredProcQuery : IStoredProcQuery
    {
        public DataTable ProcsNameList { get; set; }
        public List<string> IdList { get; }
        public Dictionary<string, string> ParamList { get; }

        public StoredProcQuery()
        {
            IdList = new List<string>();
            ParamList = new Dictionary<string, string>();
            ProcsNameList = new DataTable();

        }

        //TODO: split logic for add, get and update stored procs
        public void ParamaterNamesGet(DataTable parmsDataTable)
        {
            //uses class fileds to get param names like @filter_id_eq

            DataColumn parmName = parmsDataTable.Columns["PARAMETER_NAME"];
            DataColumn parmType = parmsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in parmsDataTable.Rows)
            {
                //For regular parameters I need to know data type to generate values
                if (!row[parmName].ToString().Contains("Id"))
                {
                    ParamList.Add(row[parmName].ToString(), row[parmType].ToString());
                }
                else
                {
                    IdList.Add(row[parmName].ToString());
                }
                //Console.WriteLine(parmRow[parmTypeDataColumn].ToString());
            }
        }

        public string QueryGet(string schemaName, string procedureName, string paramNameAndData)
        {
            var query = $"exec [{schemaName}].[{procedureName}] {paramNameAndData.TrimStart(',', ' ')}";
            return query;
        }

        public Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            foreach (var name in IdList)
            {
                if (name.Contains("Id"))
                {
                    if (name.Contains("ByUser"))
                    {
                        //tableColumnNames.Add("UserId","[core].[Users]");
                        paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));
                    }
                    else
                    {
                        var tableName = name.Replace("@filter_", "")
                                .Replace("_eq", "")
                                .Replace("@", "")
                                .Replace("Id", "");
                        var columnName = (name.Replace("@filter_", "")
                                        .Replace("_eq", "")
                                        .Replace("@", "")
                                        );

                        //tableColumnNames.Add(columnName, $"[{schema}].{tableName}s");
                        paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schemaName}].{tableName}s"));
                    }

                };

            }

            return paramColumnTable;
        }

        //TODO: Generate data for bit params. IsDeleted always 0. The rest might be random
        public void ParametersDataGenerate()
        {
            throw new System.NotImplementedException();
        }
    }
}
