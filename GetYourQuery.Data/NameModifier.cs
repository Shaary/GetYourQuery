using GetYourQuery.Core;
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Data
{
    public class NameModifier
    {
        public static List<string> specialCases = new List<string> { "@debug_mode", "@SortBy", "@SortDirection", "@filter_mf_text_like", "@page_number", "@page_size", "@PageNumber", "@PageSize", "@executed_by_procid", "@filter_DueBy_eq", "@filter_Od_eq" };
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

        public static string ColumnNameGet(string name)
        {
            return name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("_list", "")
                       .Replace("_noteq", "")
                       .Replace("_gt", "")
                       .Replace("_lt", "")
                       .Replace("@", "");
        }

        public static string PkNameGet(string name)
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

        public static List<string> ColumnNameGet(Dictionary<string, string> dict)
        {
            var names = new List<string>();

            foreach (var name in dict.Keys)
            {
                names.Add(name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("_list", "")
                       .Replace("_noteq", "")
                       .Replace("_gt", "")
                       .Replace("_lt", "")
                       .Replace("@", ""));
            }

            return names;
        }

        public static Dictionary<string, ColumnTablePair> TableAndColumnNamesGet(string schema, string procType, string procedure, List<string> IdList, DataTable TableNames)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();

            foreach (var name in IdList)
            {

                if (name.Contains("ByUser"))
                {
                    if ((procType == "Update" || procType == "Add") && name.Contains("Deleted")) //filters out DeletedByUserId column for add and update
                    {
                        continue;
                    }
                    else
                    {
                        paramColumnTable.Add(name, new ColumnTablePair("UserId", "[core].[Users]"));

                    }
                }
                else
                {
                    var tableName = NameModifier.TableNameGet(name);
                    var columnName = NameModifier.ColumnNameGet(name);

                    if (IsTableExists(tableName, schema, TableNames) && !(procType == "Add" && tableName == NameModifier.TableNameGet(procedure)))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schema}].[{tableName}]"));
                    }
                };
            }

            return paramColumnTable;
        }

        public static bool IsTableExists(string tableName, string schema, DataTable TableNames)
        {
            DataColumn tableDataColumn = TableNames.Columns["TABLE_NAME"];
            //TODO: find how to exclude schema

            if (tableDataColumn != null)
            {
                foreach (DataRow row in TableNames.Rows)
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

        public static List<string> IdParamaterNamesSet(DataTable paramsDataTable)
        {
            List<string> idList = new List<string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                if (row[paramName].ToString().Contains("Id"))
                {
                    idList.Add(row[paramName].ToString());
                }
            }
            return idList;
        }

        public static Dictionary<string, string> NonIdParamaterNamesSet(DataTable paramsDataTable)
        {
            Dictionary<string, string> nonIdDict = new Dictionary<string, string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];
            DataColumn paramType = paramsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                //For non-id parameters I need to know data type to generate values for add and update
                if (!row[paramName].ToString().Contains("DtLastUpdated") && !row[paramName].ToString().Contains("Id"))
                {
                    var result = specialCases.IndexOf(row[paramName].ToString());

                    if (result == -1)
                    {
                        nonIdDict.Add(row[paramName].ToString(), row[paramType].ToString());
                    }
                }
            }
            return nonIdDict;
        }

        public static string SpecialParamaterNamesSet(DataTable paramsDataTable)
        {
            var specialParams = "";

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                var result = specialCases.IndexOf(row[paramName].ToString());

                if (result != -1)
                {
                    specialParams += $" ,{specialCases[result]} = null";
                }
            }
            return specialParams;
        }
    }
}
