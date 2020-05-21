using GetYourQuery.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GetYourQuery.Data
{
    public class Repository : IRepository

    {
        public readonly string connString;

        public List<string> SchemaList { get; set; }
        public DataTable ParametersDataTable { get; set; }
        public DataTable TableNames { get; set; }
        public List<string> IdList { get; set; }
        public Dictionary<string, string> NonIdDict { get; set; }

        //TODO: for get stored procedures that have other tables ids create a pool of suitable ids to return data
        //example: for Equipment that has project id select equipment and project ids from project equipment table
        public Repository(string connString)
        {
            this.connString = connString;
            SchemaList = SchemaNamesGet();
            TableNames = TableNamesGet();

            IdList = new List<string>();
            NonIdDict = new Dictionary<string, string>();
        }

        public string DataGet(string procedure, string schema, string procType)
        {
            this.ParametersDataTable = ParametersTableGet(procedure, schema);
            ParamaterNamesSet(this.ParametersDataTable);

            var data = NameModifier.TableAndColumnNamesGet(schema, procType, procedure, IdList, TableNames);

            string nonIdParams = ParametersDataGet(procedure, schema, procType);
            var idParams = IdParametersDataGet(data);

            Clear();

            return idParams + nonIdParams;
        }

        public string ParametersDataGet(string procedure, string schema, string procType)
        {
            string nonIdParams;

            var tableName = NameModifier.TableNameGet(procedure);
            var pkName = NameModifier.PkNameGet(procedure);
            var pk = PrimaryKeyGet(schema, tableName, pkName);

            if (procType == "Get")
            {
                nonIdParams = NonIdParametersDataGet(NonIdDict, schema, tableName, pkName, pk);
            }
            else
            {
                nonIdParams = NonIdParametersDataGet(NonIdDict);
            }

            if (procType == "Update" || procType == "Delete")
            {
                nonIdParams = NonIdParametersDataGet(NonIdDict) + DateLastUpdatedGet(schema, tableName, pkName, pk);
            }

            return nonIdParams;
        }

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
                else if (!row[parmName].ToString().Contains("DtLastUpdated"))
                {
                    NonIdDict.Add(row[parmName].ToString(), row[parmType].ToString());
                }
            }
        }

        public DataTable ParametersTableGet(string procedure, string schema)
        {
            DataTable parmsDataTable = new DataTable();
            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                parmsDataTable = connection.GetSchema("ProcedureParameters", new string[] { null, schema, procedure });
            }
            finally
            {
                connection.Close();
            }

            return parmsDataTable;
        }

        public string IdParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable)
        {
            var nameValue = "";
            SqlConnection connection = new SqlConnection(connString);

            //finds first occurency of suitable data for a parameter
            try
            {
                connection.Open();

                foreach (var item in paramColumnTable)
                {
                    var name = item.Key;
                    var columnTable = item.Value;

                    var query = $"SELECT TOP(1) {columnTable.ColumnName} FROM {columnTable.TableName} WHERE IsDeleted = 0;";

                    var cmd = new SqlCommand(query, connection);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        nameValue += $" ,{name} = '{reader[columnTable.ColumnName]}'";
                    }
                    else
                    {
                        nameValue += $" ,{name} = NULL";
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return nameValue;
        }

        public List<string> StoredProcedureNamesGet(string schema, string database, string procType)
        {
            List<string> storedProcList = new List<string>();
            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                var query = $"SELECT SPECIFIC_NAME FROM [{database}].INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' and SPECIFIC_SCHEMA = '{schema}' and SPECIFIC_NAME like '%{procType}' ORDER BY SPECIFIC_NAME; ";

                var cmd = new SqlCommand(query, connection);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    storedProcList.Add(reader["SPECIFIC_NAME"].ToString());
                }
            }
            finally
            {
                connection.Close();
            }

            return storedProcList;
        }

        public DataTable TableNamesGet()
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                dataTable = connection.GetSchema("Tables", new string[] { null, null });
            }
            finally
            {
                connection.Close();
            }

            return dataTable;
        }

        public List<string> SchemaNamesGet()
        {
            List<string> schemaList = new List<string>();
            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                var query = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME not in ('sys', 'guest', 'INFORMATION_SCHEMA') and SCHEMA_NAME not like 'db_%' ORDER BY SCHEMA_NAME; ";

                var cmd = new SqlCommand(query, connection);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    schemaList.Add(reader["SCHEMA_NAME"].ToString());
                }
            }
            finally
            {
                connection.Close();
            }
            return schemaList;
        }

        public string RelatedParametersDataGet()
        {
            //TODO: pass list of ids like get non id function does
            // Get the first set that returns results
            throw new System.NotImplementedException();
        }

        //Function for Get stored procs only
        public string NonIdParametersDataGet(Dictionary<string, string> nonIdDict, string schema, string tableName, string pkName, string pk)
        {
            var nonIdParams = "";

            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                var paramNames = NameModifier.ColumnNameGet(nonIdDict);

                var query = $"SELECT TOP(1) {string.Join(",", paramNames)} FROM [{schema}].{tableName} WHERE {pkName} = '{pk}' and IsDeleted = 0 ORDER BY DtLastUpdated DESC";

                var cmd = new SqlCommand(query, connection);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    foreach (var par in paramNames)
                    {
                        if (reader[par].ToString() == "False")
                        {
                            nonIdParams += $" ,@filter_{par}_eq = 0";
                        }
                        else if (reader[par].ToString() == "True")
                        {
                            nonIdParams += $" ,@filter_{par}_eq = 1";
                        }
                        else
                        {
                            nonIdParams += $" ,@filter_{par}_eq = {reader[par]}";
                        }
                    }
                }
            }
            finally
            {
                connection.Close();
            }

            return nonIdParams;
        }

        //Needed for update, delete, get
        public string PrimaryKeyGet(string schema, string tableName, string pkName)
        {
            var pkId = default(Guid).ToString();

            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                var query = $"SELECT TOP(1) {pkName} FROM [{schema}].{tableName} WHERE IsDeleted = 0";

                var cmd = new SqlCommand(query, connection);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    pkId = reader[pkName].ToString();
                }
            }
            finally
            {
                connection.Close();
            }

            return pkId;
        }

        //Need this one for update and delete stored procs. Concurrency check
        public string DateLastUpdatedGet(string schema, string tableName, string pkName, string pk)
        {
            var dtLastUpdated = "";

            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                var query = $"SELECT TOP(1) DtLastUpdated FROM [{schema}].{tableName} WHERE {pkName} = '{pk}' and IsDeleted = 0 ORDER BY DtLastUpdated DESC";

                var cmd = new SqlCommand(query, connection);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dtLastUpdated = reader.GetDateTime(0).ToString("yyyy-MM-dd hh:mm:ss.fff");
                }
            }
            finally
            {
                connection.Close();
            }
            return $" ,@DtLastUpdated = '{dtLastUpdated}'";
        }

        public string NonIdParametersDataGet(Dictionary<string, string> NonIdDict)
        {
            var nonIdParamColumnTable = "";

            foreach (var item in NonIdDict)
            {
                nonIdParamColumnTable += DataGenerator.GenerateData(item.Key, item.Value);
            }
            return nonIdParamColumnTable;
        }

        public bool IsTableExists(string tableName, string schema)
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

        public void Clear()
        {
            NonIdDict.Clear();
            IdList.Clear();
            ParametersDataTable.Clear();
        }
    }
}
