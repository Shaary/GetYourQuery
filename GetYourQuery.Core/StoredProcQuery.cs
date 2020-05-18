using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GetYourQuery.Core
{
    public class StoredProcQuery : IStoredProcQuery
    {
        public readonly string storedProcName;
        public readonly string pkName;
        public readonly string schemaName;
        public readonly string tableName;

        public DataTable ProcNameTable { get; set; }
        public DataTable TableNameTable { get; set; }
        public List<string> IdList { get; } //Ids got from different tables
        public Dictionary<string, string> NonIdDict { get; }  //Non ids got from the same table
        public string PkId { get; set; }

        public StoredProcQuery(DataTable TableNameTable, string storedProcName, string schemaName)
        {
            IdList = new List<string>();
            NonIdDict = new Dictionary<string, string>();
            ProcNameTable = new DataTable();

            //need it to check for non-existing tables that will show up from params like ExternalUniqueId
            this.TableNameTable = TableNameTable;
            this.storedProcName = storedProcName;
            this.pkName = PkNameGet(storedProcName);
            this.schemaName = schemaName;
            this.tableName = TableNameGet(storedProcName);
        }

        //TODO: split logic for add, get and update stored procs
        public virtual void ParamaterNamesSet(DataTable parmsDataTable)
        {
            DataColumn parmName = parmsDataTable.Columns["PARAMETER_NAME"];
            DataColumn parmType = parmsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in parmsDataTable.Rows)
            {
                if (row[parmName].ToString().Contains("Id"))
                {
                    IdList.Add(row[parmName].ToString());
                }
                //For non-id parameters I need to know data type to generate values for add and update
                else
                {
                    NonIdDict.Add(row[parmName].ToString(), row[parmType].ToString());
                }
            }
        }

        public virtual string QueryGet(string schemaName, string paramNameAndData)
        {
            var nonIdParamColumnTable = ParametersDataGenerate();

            var query = $"exec [{schemaName}].[{storedProcName}] {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")} {nonIdParamColumnTable.Replace(",", Environment.NewLine + ",")}";
            return query;
        }

        public virtual Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            foreach (var name in IdList)
            {

                if (name.Contains("ByUser"))
                {
                    paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));
                }
                else
                {
                    var tableName = TableNameGet(name);
                    var columnName = ColumnNameGet(name);

                    if (IsTableExists(tableName))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schemaName}].[{tableName}]"));
                    }
                };
            }

            return paramColumnTable;
        }

        private string ParametersDataGenerate()
        {
            var nonIdParamColumnTable = "";

            foreach (var item in NonIdDict)
            {
                nonIdParamColumnTable += DataGenerator.GenerateData(item.Key, item.Value);
            }
            return nonIdParamColumnTable;
        }

        public bool IsTableExists(string tableName)
        {
            DataColumn tableDataColumn = TableNameTable.Columns["TABLE_NAME"];

            if (tableDataColumn != null)
            {
                foreach (DataRow row in TableNameTable.Rows)
                {
                    var tabName = row[tableDataColumn].ToString();
                    if (tabName == tableName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string TableNameGet(string name)
        {
            var tableName = name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("@", "")
                       .Replace("Id", "")
                       .Replace("usp_", "")
                       .Replace("sGet", "")
                       .Replace("sAdd", "")
                       .Replace("sDelete", "")
                       .Replace("Stats", "")
                       .Replace("sUpdate", "");

            if (tableName.EndsWith("y"))
            {
                tableName = tableName.Replace("y", "ie");
            }

            tableName += "s";

            return tableName;
        }

        public string ColumnNameGet(string name)
        {
            return name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("@", "");
        }

        public string PkNameGet(string name)
        {
            var pkName = name.Replace("usp_", "")
                       .Replace("sGet", "")
                       .Replace("sAdd", "")
                       .Replace("sDelete", "")
                       .Replace("Stats", "")
                       .Replace("sUpdate", "");
            //Trims names like companies, facilities
            if (pkName.EndsWith("ie"))
            {
                pkName = pkName.Replace("ie", "y");
            }
            
            pkName += "Id";

            return pkName;
        }

    }
}
