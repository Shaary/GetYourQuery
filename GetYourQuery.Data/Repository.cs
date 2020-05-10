using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GetYourQuery.Core;

namespace GetYourQuery.Core
{
    public class Repository : IRepository

    {
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

                    var query = $"SELECT TOP(1) {columnTable.ColumnName} FROM {columnTable.TableName} WHERE IsDeleted = 0";

                    var cmd = new SqlCommand(query, db);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        nameValue += $" ,{name} = '{reader[columnTable.ColumnName]}'";
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
            DataTable parmsDataTable = new DataTable();

            try
            {
                db.Open();

                parmsDataTable = db.GetSchema("Procedures", new string[] { null, schema });
            }
            finally
            {
                db.Close();
            }

            return parmsDataTable;
        }
    }
}
