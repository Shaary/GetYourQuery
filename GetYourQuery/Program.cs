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

                storedProcQuery.ProcNameTable = procsTable;

                var isProcExists = storedProcQuery.IsNameExists(procName);

                repository.SchemaNamesGet();

                if (isProcExists)
                {
                    storedProcQuery.ParamaterNamesGet(dataTable);
                    storedProcQuery.ParametersDataGenerate();

                    //Sets storedProcsQuery tableNameTable to check for non-existing tables that will show up from params like ExternalUniqueId
                    var tablesTable = repository.TableNamesGet(schema);
                    storedProcQuery.TableNameTable = tablesTable;

                    var dict = storedProcQuery.TableAndColumnNamesGet(schema);

                    var data = repository.ParametersDataGet(dict);

                    var query = storedProcQuery.QueryGet(schema, procName, data);

                    Console.WriteLine(query);
                }
                else
                {
                    Console.WriteLine("Sorry, I wasn't able to find the stored procedure...");
                }

                Console.WriteLine("Continue? ");
                c = Console.ReadLine();
            }
            

        }
    }
}
