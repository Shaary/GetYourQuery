using GetYourQuery.Core;
using System.Collections.Generic;
using System.Data;

namespace GetYourQuery.Data
{
    public class NameModifier
    {
        public static List<string> FsSpecialCases = new List<string> { "@debug_mode", "@SortBy", "@SortDirection", "@filter_mf_text_like", "@page_number", "@page_size", "@PageNumber", "@PageSize", "@executed_by_procid", "@filter_DueBy_eq", "@filter_Od_eq", "@filter_ExternalSourceUniqueId_eq", "@filter_ExternalUniqueId_eq", "@filter_LastUpdatedByAppName_eq" };
        public static List<string> SnakeSpecialCases = new List<string> { "@debug_mode", "@sort_by", "@sort_direction", "@filter_mf_text_like", "@page_number", "@page_size", "@page_number", "@page_size", "@executed_by_procid", "@filter_DueBy_eq", "@filter_Od_eq" };
        public static string TableNameGet(string name)
        {
            var tableName = name.Replace("@filter_", "")
                                .Replace("_eq", "")
                                .Replace("@", "")
                                .Replace("Id", "")
                                .Replace("_list", "")
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

            if (tableName.EndsWith("s"))
            {
                tableName += "e";
            }
            tableName += "s";

            return tableName;
        }

        internal static List<string> UserParamaterNamesSet(DataTable paramsDataTable)
        {
            var userIdList = new List<string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                if (row[paramName].ToString().Contains("UserId") && !row[paramName].ToString().Contains("list"))
                {
                    var result = FsSpecialCases.IndexOf(row[paramName].ToString());

                    if (result == -1)
                    {
                        userIdList.Add(row[paramName].ToString());
                    }
                }
            }
            return userIdList;
        }

        public static string ColumnNameGet(string name)
        {
            return name.Replace("@filter_", "")
                       .Replace("_eq", "")
                       .Replace("_list", "")
                       .Replace("_noteq", "")
                       .Replace("_gt", "")
                       .Replace("_lt", "")
                       .Replace("SelectedBy", "")
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

        public static string UserPkNameGet(string name)
        {
            var pkName = name.Replace("@filter_", "")
                             .Replace("_eq", "")
                             .Replace("_list", "")
                             .Replace("_noteq", "")
                             .Replace("_gt", "")
                             .Replace("_lt", "")
                             .Replace("SelectedBy", "")
                             .Replace("@", "");

            pkName = pkName.Substring(pkName.LastIndexOf("User"));

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

        public static Dictionary<string, ColumnTablePair> TableAndColumnNamesSet(string schema, string procType, string procedure, List<string> IdList, DataTable TableNames)
        {
            var paramColumnTable = new Dictionary<string, ColumnTablePair>();
            //string usersSchema = UsersSchemaNameGet(TableNames);

            foreach (var name in IdList)
            {
                if (!name.Contains("ByUserId"))
                {
                    var tableName = NameModifier.TableNameGet(name);
                    var columnName = NameModifier.ColumnNameGet(name);

                    if (IsTableExists(tableName, schema, TableNames) && !(procType == "Add" && tableName == NameModifier.TableNameGet(procedure)))
                    {
                        paramColumnTable.Add(name, new ColumnTablePair(columnName, $"[{schema}].[{tableName}]"));
                    }
                }
            }

            return paramColumnTable;
        }

        public static string UsersSchemaNameSet(DataTable TableNames)
        {
            var usersSchema = "";
            DataRowCollection rows = TableNames.Rows;

            var schemaColumnName = "TABLE_SCHEMA";
            var tableColumnName = "TABLE_NAME";

            var result = TableNames
                            .AsEnumerable()
                            .Where(row => row.Field<string>(tableColumnName) == "Users");

            foreach (var item in result)
            {
                usersSchema = item[schemaColumnName].ToString();
                break; //I need the first one. TODO: check if schema for users in 'core", "common", "dbo"
            }

            return usersSchema;
        }

        public static List<string> UserIdNamesSet(List<string> idList, string procType)
        {
            var userIds = new List<string>();

            foreach (var name in idList)
            {
                if ((procType == "Update" || procType == "Add") && name.Contains("Deleted")) //filters out DeletedByUserId column for add and update
                {
                    continue;
                }
                else
                {
                    userIds.Add(name);
                }
            }
            return userIds;
        }

        public static bool IsTableExists(string tableName, string schema, DataTable TableNames)
        {
            DataColumn tableDataColumn = TableNames.Columns["TABLE_NAME"];
            DataColumn schemaDataColumn = TableNames.Columns["TABLE_SCHEMA"];

            if (tableDataColumn != null)
            {
                foreach (DataRow row in TableNames.Rows)
                {
                    var tabName = row[tableDataColumn].ToString();
                    var schName = row[schemaDataColumn].ToString();
                    if (tabName == tableName && schName == schema)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<string> IdParamaterNamesSet(DataTable paramsDataTable)
        {
            var idList = new List<string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                if (row[paramName].ToString().Contains("Id") && !row[paramName].ToString().Contains("UserId") && !row[paramName].ToString().Contains("list"))
                {
                    var result = FsSpecialCases.IndexOf(row[paramName].ToString());

                    if (result == -1)
                    {
                        idList.Add(row[paramName].ToString());
                    }
                }
            }
            return idList;
        }

        public static Dictionary<string, string> ListIdParamaterNamesSet(DataTable paramsDataTable)
        {
            var listIdDict = new Dictionary<string, string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];
            DataColumn paramType = paramsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                if (row[paramName].ToString().Contains("Id") && row[paramName].ToString().Contains("list"))
                {
                    listIdDict.Add(row[paramName].ToString(), row[paramType].ToString());
                }
            }
            return listIdDict;
        }

        public static Dictionary<string, string> NonIdParamaterNamesSet(DataTable paramsDataTable)
        {
            //For non-id parameters I need to know data type to generate values for add and update
            var nonIdDict = new Dictionary<string, string>();

            DataColumn paramName = paramsDataTable.Columns["PARAMETER_NAME"];
            DataColumn paramType = paramsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in paramsDataTable.Rows)
            {
                if (!row[paramName].ToString().Contains("DtLastUpdated") && !row[paramName].ToString().Contains("Id"))
                {
                    var result = FsSpecialCases.IndexOf(row[paramName].ToString());

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
                var result = FsSpecialCases.IndexOf(row[paramName].ToString());

                if (result != -1)
                {
                    specialParams += $" ,{FsSpecialCases[result]} = null";
                }
            }
            return specialParams;
        }
    }
}
