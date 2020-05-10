using GetYourQuery.Core;
using System;

namespace GetYourQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=AmazingDb; Integrated Security=True;";

            var c = "y";
            while (c != "n")
            {
                Console.WriteLine("Enter schema name: ");
                var schema = Console.ReadLine();
                Console.WriteLine("Enter stored procedure name: ");
                var procName = Console.ReadLine();

                // classes init
                var repository = new Repository(connectionString);
                var storedProcQuery = new StoredProcQuery();

                var dataTable = repository.ParametersTableGet(procName, schema);
                var procsTable = repository.StoredProcedureNamesGet(schema);

                storedProcQuery.ProcsNameList = procsTable;

                storedProcQuery.ParamaterNamesGet(dataTable);

                var dict = storedProcQuery.TableAndColumnNamesGet(schema);

                var data = repository.ParametersDataGet(dict);

                var query = storedProcQuery.QueryGet(schema, procName, data);

                Console.WriteLine(query);

                Console.WriteLine("Continue? ");
                c = Console.ReadLine();
            }
            

        }
    }
}
