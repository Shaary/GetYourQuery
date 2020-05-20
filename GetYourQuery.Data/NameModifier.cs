using System;
using System.Collections.Generic;
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
                       .Replace("@", ""));
            }

            return names;
        }
    }
}
