﻿using GetYourQuery.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace GetYourQuery.Data
{
    public class NameModifier
    {
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

        public static List<string> IdParamaterNamesSet(DataTable parmsDataTable)
        {
            List<string> idList = new List<string>();

            DataColumn parmName = parmsDataTable.Columns["PARAMETER_NAME"];

            foreach (DataRow row in parmsDataTable.Rows)
            {
                if (row[parmName].ToString().Contains("Id"))
                {
                    idList.Add(row[parmName].ToString());
                }
            }
            return idList;
        }

        public static Dictionary<string, string> NonIdParamaterNamesSet(DataTable parmsDataTable)
        {
            Dictionary<string, string> nonIdDict = new Dictionary<string, string>();

            DataColumn parmName = parmsDataTable.Columns["PARAMETER_NAME"];
            DataColumn parmType = parmsDataTable.Columns["DATA_TYPE"];

            foreach (DataRow row in parmsDataTable.Rows)
            {
                //For non-id parameters I need to know data type to generate values for add and update
                if (!row[parmName].ToString().Contains("DtLastUpdated") && !row[parmName].ToString().Contains("Id"))
                {
                    nonIdDict.Add(row[parmName].ToString(), row[parmType].ToString());
                }
            }
            return nonIdDict;
        }
    }
}
