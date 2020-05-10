﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using GetYourQuery.Core;

namespace GetYourQuery.Core
{
    public class Repository : IRepository

    {
        private SqlConnection db;
        private readonly string procName;

        public Repository(string connString, string procName = null)
        {
            db = new SqlConnection(connString);
            this.procName = procName;
        }

        public DataTable ParametersTableGet()
        {
            DataTable parmsDataTable = new DataTable();

            try
            {
                db.Open();

                parmsDataTable = db.GetSchema("ProcedureParameters", new string[] { null, null, procName });
            }
            finally
            {
                db.Close();
            }

            return parmsDataTable;

        }

        public string ParametersDataGet(Dictionary<string, ColumnTablePair> paramColumnTable)
        {
            //var data = new Dictionary<string, string>();
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

                //for (int i = 0; i < tableNames.Count; i++)
                //{
                //    var query = $"SELECT TOP(1) {columnNames[i]} FROM {tableNames[i]} WHERE IsDeleted = 0";

                //    var cmd = new SqlCommand(query, db);

                //    using SqlDataReader reader = cmd.ExecuteReader();
                //    while (reader.Read())
                //    {
                //        nameValue += $" ,{paramNames[i]} = '{reader[columnNames[i]]}'";
                //        //data.Add(paramNames[i], reader[columnNames[i]].ToString());
                //    }
                //}
            }
            finally
            {
                db.Close();
            }
            return nameValue;
        }

        public string ParametersDataGenerate(Dictionary<string, string> paramNames)
        {
            throw new NotImplementedException();
        }

    }
}
