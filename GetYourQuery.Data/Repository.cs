using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GetYourQuery.Core
{
    public class Repository : IRepository

    {
        private readonly string connString;

        //TODO: for get stored procedures that have other tables ids create a pool of suitable ids to return data
        //example: for Equipment that has project id select equipment and project ids from project equipment table

        public Repository(string connString)
        {
            this.connString = connString;
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

        public string ParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable)
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

                var query = $"SELECT SPECIFIC_NAME FROM [{database}].INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' and SPECIFIC_SCHEMA = '{schema}' and SPECIFIC_NAME like '%{procType}'; ";

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

        public DataTable TableNamesGet(string schema)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(connString);

            try
            {
                connection.Open();

                dataTable = connection.GetSchema("Tables", new string[] { null, schema });
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

                var query = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME not in ('sys', 'guest', 'INFORMATION_SCHEMA') and SCHEMA_NAME not like 'db_%'; ";

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
    }
}
