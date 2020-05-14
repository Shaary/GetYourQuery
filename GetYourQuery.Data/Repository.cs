using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GetYourQuery.Core
{
    public class Repository : IRepository

    {
        //TODO: for get stored procedures that have other tables ids create a pool of suitable ids to return data
        //example: for Equipment that has project id select equipment and project ids from project equipment table
        private SqlConnection db;

        public Repository(string connString)
        {
            db = new SqlConnection(connString);
        }

        public DataTable ParametersTableGet(string procedure, string schema)
        {
            DataTable parmsDataTable = new DataTable();

            try
            {
                db.Open();

                parmsDataTable = db.GetSchema("ProcedureParameters", new string[] { null, schema, procedure });
            }
            finally
            {
                db.Close();
            }

            return parmsDataTable;

        }

        public string ParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable)
        {
            var nameValue = "";
            //finds first occurency of suitable data for a parameter
            try
            {
                db.Open();

                foreach (var item in paramColumnTable)
                {
                    var name = item.Key;
                    var columnTable = item.Value;

                    var query = $"SELECT TOP(1) {columnTable.ColumnName} FROM {columnTable.TableName} WHERE IsDeleted = 0;";

                    var cmd = new SqlCommand(query, db);

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
                db.Close();
            }
            return nameValue;
        }

        public DataTable StoredProcedureNamesGet(string schema)
        {
            DataTable dataTable = new DataTable();

            try
            {
                db.Open();

                dataTable = db.GetSchema("Procedures", new string[] { null, schema });
            }
            finally
            {
                db.Close();
            }

            return dataTable;
        }

        public DataTable TableNamesGet(string schema)
        {
            DataTable dataTable = new DataTable();

            try
            {
                db.Open();

                dataTable = db.GetSchema("Tables", new string[] { null, schema });
            }
            finally
            {
                db.Close();
            }

            return dataTable;
        }

        public List<string> SchemaNamesGet()
        {
            List<string> schemaList = new List<string>();
            try
            {
                db.Open();

                var query = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA where SCHEMA_NAME not in ('sys', 'guest', 'INFORMATION_SCHEMA') and SCHEMA_NAME not like 'db_%'; ";

                var cmd = new SqlCommand(query, db);

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    schemaList.Add(reader["SCHEMA_NAME"].ToString());
                }
            }
            finally
            {
                db.Close();
            }
            return schemaList;
        }
    }
}
