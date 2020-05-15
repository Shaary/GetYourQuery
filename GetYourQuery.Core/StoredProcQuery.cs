using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GetYourQuery.Core
{
    public class StoredProcQuery : IStoredProcQuery
    {
        const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public DataTable ProcNameTable { get; set; }
        public DataTable TableNameTable { get; set; }
        public List<string> IdList { get; }
        public Dictionary<string, string> ParamList { get; }
        private string nonIdParamColumnTable;

        public StoredProcQuery()
        {
            IdList = new List<string>();
            ParamList = new Dictionary<string, string>();
            ProcNameTable = new DataTable();
            TableNameTable = new DataTable();
            nonIdParamColumnTable = "";

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
            }
        }

        public string QueryGet(string schemaName, string procedureName, string paramNameAndData)
        {
            var query = $"exec [{schemaName}].[{procedureName}] {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")} {nonIdParamColumnTable.Replace(",", Environment.NewLine + ",")}";
            return query;
        }

        public virtual Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schemaName)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            foreach (var name in IdList)
            {
                if (name.Contains("Id"))
                {
                    if (name.Contains("ByUser"))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));
                    }
                    else
                    {
                        var tableName = name.Replace("@filter_", "")
                                .Replace("_eq", "")
                                .Replace("@", "")
                                .Replace("Id", "") + "s";
                        var columnName = (name.Replace("@filter_", "")
                                        .Replace("_eq", "")
                                        .Replace("@", "")
                                        );

                        if (IsTableExists(tableName))
                        {
                            paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schemaName}].[{tableName}]"));
                        }
                    }

                };

            }

            return paramColumnTable;
        }

        //TODO: Generate data for bit params. IsDeleted always 0. The rest might be random
        public void ParametersDataGenerate()
        {
            var random = new Random();

            foreach (var item in ParamList)
            {
                Console.WriteLine(item.Value);
                switch (item.Value)
                {
                    case "bit":
                        if (item.Key != "IsDeleted")
                        {
                            nonIdParamColumnTable += $" ,{item.Key} = {random.Next(0, 1)}";
                        }
                        else
                        {
                            nonIdParamColumnTable += $" ,{item.Key} = 0";
                        }
                        break;
                    case "decimal":
                        var randomDouble = random.Next(-4, 4) + (0.6689);
                        nonIdParamColumnTable += $" ,{item.Key} = {randomDouble}";
                        break;
                    case "int":
                        nonIdParamColumnTable += $" ,{item.Key} = {random.Next(0, 100)}";
                        break;
                    case "nvarchar":
                        var length = random.Next(0, 25);
                        var randomString = new string(Enumerable.Repeat(CHARS, length)
                          .Select(s => s[random.Next(s.Length)]).ToArray());
                        nonIdParamColumnTable += $" ,{item.Key} = '{randomString}'";
                        break;
                    default:
                        {
                            nonIdParamColumnTable += $" ,{item.Key} = NULL";
                            break;
                        }

                }
            }
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
