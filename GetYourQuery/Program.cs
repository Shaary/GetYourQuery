using GetYourQuery.Core;
using System;

namespace GetYourQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AmazingDb; Integrated Security=True;";

            //Console.WriteLine("Enter schema name: ");
            //var schema = Console.ReadLine();
            //Console.WriteLine("Enter stored procedure name: ");
            //var procName = Console.ReadLine();

            var repository = new Repository(connectionString, "usp_AccessTypesGet");

            var storedProcQuery = new StoredProcQuery();

            var dataTable = repository.ParametersTableGet();

            storedProcQuery.ParamaterNamesGet(dataTable);

            var dict = storedProcQuery.TableAndColumnNamesGet("fs");


            //foreach (string param in parameters)
            //{
            //    Console.WriteLine(param);
            //}

            var data = repository.ParametersDataGet(dict);



            var query = storedProcQuery.QueryGet("fs", "usp_AccessTypesGet", data);

            Console.WriteLine(query);

            //Test methods
            //IDbConnection db = new SqlConnection(connectionString);
            //var data = db.Query("select * from fs.AccessTypes").ToList();

            //var parametersNames = repository.ParametersNamesGet();
            //var parametersData = repository.ParametersDataGet(parametersNames);

            //var query = new StoredProcQuery(procName, parametersNames, parametersData);

            //Console.WriteLine(query);
        }
    }
}
