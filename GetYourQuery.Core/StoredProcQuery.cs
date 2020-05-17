using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GetYourQuery.Core
{
    public class StoredProcQuery : IStoredProcQuery
    {
        public DataTable ProcNameTable { get; set; }
        public DataTable TableNameTable { get; set; }
        //Ids got from different tables
        public List<string> IdList { get; }
        //Non ids got from the same table
        public Dictionary<string, string> NonIdDict { get; }

        public StoredProcQuery(DataTable TableNameTable)
        {
            IdList = new List<string>();
            NonIdDict = new Dictionary<string, string>();
            ProcNameTable = new DataTable();

            //need it to check for non-existing tables that will show up from params like ExternalUniqueId
            this.TableNameTable = TableNameTable;

        }

        //TODO: split logic for add, get and update stored procs
        public virtual void ParamaterNamesSet(DataTable parmsDataTable)
        {
            //uses class fileds to get param names like @filter_id_eq

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

        public virtual string QueryGet(string schemaName, string procedureName, string paramNameAndData)
        {
            var nonIdParamColumnTable = ParametersDataGenerate();

            var query = $"exec [{schemaName}].[{procedureName}] {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")} {nonIdParamColumnTable.Replace(",", Environment.NewLine + ",")}";
            return query;
        }

        public virtual Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName, string storedProcName = null)
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

        public string TableNameGet(string name)
        {
            return name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("@", "")
                       .Replace("Id", "") + "s";
        }

        public string ColumnNameGet(string name)
        {
            return name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("@", "");
        }

        public string ParametersDataGenerate()
        {
            var nonIdParamColumnTable = "";

            foreach (var item in NonIdDict)
            {
                nonIdParamColumnTable += DataGenerator.GenerateData(item.Key, item.Value);
            }
            return nonIdParamColumnTable;
        }

        //Checks if entered value is a valid name
        public bool IsNameExists(string procName)
        {
            DataColumn procedureDataColumn = ProcNameTable.Columns["ROUTINE_NAME"];

            if (procedureDataColumn != null)
            {
                foreach (DataRow row in ProcNameTable.Rows)
                {
                    var procedureName = row[procedureDataColumn].ToString();
                    if (procedureName == procName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsTableExists(string tableName)
        {
            DataColumn tableDataColumn = TableNameTable.Columns["TABLE_NAME"];

            if (tableDataColumn != null)
            {
                foreach (DataRow row in TableNameTable.Rows)
                {
                    var tabName = row[tableDataColumn].ToString();
                    //Console.WriteLine(tabName);
                    if (tabName == tableName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
