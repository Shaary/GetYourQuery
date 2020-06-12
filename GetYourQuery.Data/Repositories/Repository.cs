using GetYourQuery.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GetYourQuery.Data
{
    public class Repository

    {
        public readonly string connString;

        public List<string> SchemaList { get; set; }
        public DataTable ParametersDataTable { get; set; }
        public DataTable TableNames { get; set; }
        public DataTable CommonIdsTable { get; }

        //TODO: verify that get stored procedure returns results and if not try the next one
        //TODO: create filters
        //TODO: show parameters as checkboxes
        //TODO: create separate NameModifier classes for camel and snake cases
        public Repository(string connString)
        {
            this.connString = connString;
            SchemaList = SchemaNamesGet();
            TableNames = TableNamesGet();
            CommonIdsTable = CommonDataTableGet();

        }

        public string DataGet(string procedure, string schema, string procType)
        {
            ParametersDataTable = ParametersTableGet(procedure, schema);
            var idList = NameModifier.IdParamaterNamesSet(ParametersDataTable);
            var userList = NameModifier.UserParamaterNamesSet(ParametersDataTable);
            var nonIdDict = NameModifier.NonIdParamaterNamesSet(ParametersDataTable);
            var userIdList = NameModifier.UserIdNamesSet(userList, procType);
            var listIdDict = NameModifier.ListIdParamaterNamesSet(ParametersDataTable);
            var userSchema = NameModifier.UsersSchemaNameSet(TableNames);
            var data = NameModifier.TableAndColumnNamesSet(schema, procType, procedure, idList, TableNames);

            var allParams = AllParametersGet(procedure, schema, procType, idList, nonIdDict, userIdList, userSchema, data);

            var idListParams = IdListParametersGet(listIdDict, schema);

            var query = QueryGet(schema, procedure, allParams, idListParams);

            Clear();

            return query;
        }

        private string AllParametersGet(string procedure, string schema, string procType, List<string> idList, Dictionary<string, string> nonIdDict, List<string> userIdList, string userSchema, Dictionary<string, ColumnTablePair> data)
        {
            var nonIdParams = "";
            if (nonIdDict.Count > 0)
            {
                nonIdParams = ParametersDataGet(procedure, schema, procType, nonIdDict);
            }

            var idParams = "";
            if (procType == "Get")
            {
                idParams = RelatedParametersDataGet(idList, CommonIdsTable, schema);
            }
            else
            {
                idParams = IdParametersDataGet(data);
            }

            var specialParams = NameModifier.SpecialParamaterNamesSet(ParametersDataTable);

            var userIdParams = "";
            if (userIdList.Count > 0)
            {
                userIdParams = UserIdsGet(userIdList, userSchema);
            }

            return nonIdParams + idParams + specialParams + userIdParams;

        }

        public string ParametersDataGet(string procedure, string schema, string procType, Dictionary<string, string> nonIdDict)
        {
            string nonIdParams = "";

            var tableName = NameModifier.TableNameGet(procedure);
            if (IsTableExists(tableName, schema) && !tableName.Contains("Status"))
            {
                var pkName = NameModifier.PkNameGet(procedure);
                var pk = PrimaryKeyIdGet(schema, tableName, pkName);

                nonIdParams = NonIdParametersDataGet(schema, procType, tableName, pkName, pk, nonIdDict);
            }
            return nonIdParams;
        }

        private string NonIdParametersDataGet(string schema, string procType, string tableName, string pkName, string pk, Dictionary<string, string> nonIdDict)
        {
            string nonIdParams;
            if (procType == "Get")
            {
                nonIdParams = NonIdParametersDataGet(nonIdDict, schema, tableName, pkName, pk);
            }
            else
            {
                nonIdParams = NonIdParametersDataGet(nonIdDict);
            }

            if (procType == "Update" || procType == "Delete")
            {
                nonIdParams = NonIdParametersDataGet(nonIdDict) + DateLastUpdatedGet(schema, tableName, pkName, pk);
            }

            return nonIdParams;
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

                    var query = $"SELECT TOP(1) {columnTable.ColumnName} FROM {columnTable.TableName} WHERE IsDeleted = 0 ORDER BY NEWID();";

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

        public string IdParametersDataGet(string paramName, string schema)
        {
            var paramId = "";

            var tableName = NameModifier.TableNameGet(paramName);

            if (IsTableExists(tableName, schema))
            {
                SqlConnection connection = new SqlConnection(connString);

                //finds first occurency of suitable data for a parameter
                try
                {
                    connection.Open();

                    var columnName = NameModifier.ColumnNameGet(paramName);

                    var query = $"SELECT TOP(1) {columnName} FROM {schema}.{tableName} WHERE IsDeleted = 0 ORDER BY NEWID();";

                    var cmd = new SqlCommand(query, connection);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        paramId = reader[columnName].ToString();
                    }
                    else
                    {
                        paramId = "NULL";
                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            return paramId;
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

        public List<string> IdListParametersGet(Dictionary<string, string> listIdDict, string schema)
        {
            var idListParams = new List<string>();

            if (listIdDict.Count > 0)
            {
                var declareString = "";
                var listId = "";

                var i = 0;

                foreach (var key in listIdDict.Keys)
                {
                    var tableName = NameModifier.TableNameGet(key);

                    if (IsTableExists(tableName, schema))
                    {
                        declareString += $" declare @p{i} dbo.{listIdDict[key]} ";
                        listId += $" ,{key} = @p{i} ";
                        var columnName = NameModifier.ColumnNameGet(key);

                        var query = $"SELECT TOP(3) {columnName} from [{schema}].{tableName}";

                        SqlConnection connection = new SqlConnection(connString);
                        try
                        {
                            connection.Open();

                            var cmd = new SqlCommand(query, connection);

                            using SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                declareString += $" ,insert into @p{i} values ('{reader[columnName]}') ";
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    i++;
                }
                idListParams.Add(declareString);
                idListParams.Add(listId);
            }
            return idListParams;
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

            DataColumnCollection columns = dataTable.Columns;
            var k = "";
            foreach (var column in columns)
            {
                k += "   " + column;
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

        public string UserIdsGet(List<string> userIdsList, string userSchema)
        {
            var userParams = "";
            if (userIdsList.Count > 0)
            {
                var userId = "";
                var userPk = NameModifier.UserPkNameGet(userIdsList[0]);
                SqlConnection connection = new SqlConnection(connString);
                try
                {
                    connection.Open();

                    var query = $"SELECT TOP(1) {userPk} FROM [{userSchema}].[Users]";

                    var cmd = new SqlCommand(query, connection);

                    using SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userId = reader["UserId"].ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }

                foreach (var name in userIdsList)
                {
                    userParams += $" ,{name} = '{userId}' ";
                }
            }
            return userParams;
        }

        public string RelatedParametersDataGet(List<string> idList, DataTable dataTable, string schema)
        {
            DataColumnCollection columns = dataTable.Columns;
            DataRowCollection rows = dataTable.Rows;
            var relatedIds = "";

            var amount = rows.Count;
            if (amount > 0)
            {
                var random = new Random();

                DataRow row = rows[random.Next(0, amount - 1)];

                foreach (var parameter in idList)
                {
                    var columnName = NameModifier.ColumnNameGet(parameter);
                    var id = "";
                    if (columns.Contains(columnName))
                    {
                        id = row[columnName].ToString();

                    }
                    if (String.IsNullOrEmpty(id))
                    {
                        id = IdParametersDataGet(parameter, schema);
                        relatedIds += $" ,{parameter} = '{id}' ";
                    }
                    else
                    {
                        relatedIds += $" ,{parameter} = '{id}' ";
                    }
                }
            }
            return relatedIds;
        }

        public DataTable CommonDataTableGet()
        {
            var query = "select distinct * from fs.Tasks t inner join fs.ProjectEquipments pe on t.ProjectEquipmentId = pe.ProjectEquipmentId ";
            query += " inner join fs.Equipments e on pe.EquipmentId = e.EquipmentId ";
            query += " inner join fs.Projects p on pe.ProjectId = p.ProjectId ";
            query += " inner join fs.Facilities f on e.FacilityId = f.FacilityId ";
            query += " inner join fs.InspectionPoints ips on e.EquipmentId = ips.EquipmentId ";


            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }
                }
            }
            return dataTable;
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
                            nonIdParams += $" ,@filter_{par}_eq = '{reader[par]}'";
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
        public string PrimaryKeyIdGet(string schema, string tableName, string pkName)
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
                    dtLastUpdated = reader.GetDateTime(0).ToString("yyyy-MM-dd HH:mm:ss.fff");
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
            ParametersDataTable.Clear();
        }

        public static string QueryGet(string schemaName, string storedProcName, string paramNameAndData, List<string> listParams)
        {
            var query = "";
            if (listParams.Count > 0)
            {
                var listDeclaration = listParams[0].Replace(",", Environment.NewLine);
                query = listDeclaration + Environment.NewLine;
                query += $"exec [{schemaName}].[{storedProcName}] {Environment.NewLine} {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")} {listParams[1].Replace(",", Environment.NewLine + ",")}";
            }
            else
            {
                query = $"exec [{schemaName}].[{storedProcName}] {Environment.NewLine} {paramNameAndData.TrimStart(',', ' ').Replace(",", Environment.NewLine + ",")}";
            }
            return query;
        }


    }
}
