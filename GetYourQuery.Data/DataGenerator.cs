using System;
using System.Collections.Generic;
using System.Linq;

namespace GetYourQuery.Core
{
    public class DataGenerator
    {

        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string NUMS = "0123456789";

        private static List<string> address = new List<string>() { "Elm Street 35", "Ashenvale 305", "Orgrimmar 1", "Cheese store 1", "Forge #6", "Street 404",
                                    "Middle of nowhere 5", "Sherwood tree 67", "Animal Crossing 6" , "Art gallery 8" };
        private static List<string> city = new List<string> { "Stormwind", "Ironforge", "Denbery", "Calgary", "Ottawa", "Undercity", "Derry", "Moscow", "Dalanaar", "Zul Gurub" };
        private static List<string> country = new List<string> { "Canada", "Russia", "Azeroth", "USA", "Outland", "Northrend", "Pandaria", "Mongolia", "Ireland", "New Zeland" };
        private static List<string> province = new List<string> { "Alberta", "Quebec", "Arizona", "California", "Goldshire", "Hobbiton", "Mordor", "Texas", "Ontario", "BC" };


        public DataGenerator()
        {
        }

        public static string GenerateData(string paramName, string paramType)
        {
            var random = new Random();
            var nonIdParamColumnTable = "";

            switch (paramType)
            {
                case "bit":
                    if (paramName == "IsDeleted")
                    {
                        nonIdParamColumnTable += $" ,{paramName} = 0";
                    }
                    else
                    {
                        nonIdParamColumnTable += $" ,{paramName} = {random.Next(0, 1)}";
                    }
                    break;

                case "decimal":
                    var randomDouble = random.Next(-4, 4) + (0.6689);
                    nonIdParamColumnTable += $" ,{paramName} = {randomDouble}";
                    break;

                case "int":
                    nonIdParamColumnTable += $" ,{paramName} = {random.Next(0, 10)}";
                    break;

                case "nvarchar":
                    var length = random.Next(0, 10);
                    switch (paramName)
                    {
                        case ("@Code"):
                            nonIdParamColumnTable += $" ,{paramName} = '{length}'";
                            break;
                        case ("@Position"):
                            nonIdParamColumnTable += $" ,{paramName} = '{length}'";
                            break;
                        case ("@Address1"):
                            nonIdParamColumnTable += $" ,{paramName} = '{address[length]}'";
                            break;
                        case ("@Address2"):
                            nonIdParamColumnTable += $" ,{paramName} = '{address[length]}'";
                            break;
                        case ("@City"):
                            nonIdParamColumnTable += $" ,{paramName} = '{city[length]}'";
                            break;
                        case ("@StateProvince"):
                            nonIdParamColumnTable += $" ,{paramName} = '{province[length]}'";
                            break;
                        case ("@Country"):
                            nonIdParamColumnTable += $" ,{paramName} = '{country[length]}'";
                            break;
                        case ("@Phone"):
                            var phoneNum = new string(Enumerable.Repeat(NUMS, 9)
                                                      .Select(s => s[random.Next(s.Length)]).ToArray());
                            nonIdParamColumnTable += $" ,{paramName} = '+1{phoneNum}'";
                            break;

                        default:
                            {
                                var randomString = new string(Enumerable.Repeat(CHARS, length)
                                                      .Select(s => s[random.Next(s.Length)]).ToArray());
                                nonIdParamColumnTable += $" ,{paramName} = '{randomString}'";
                                break;
                            }
                    }
                    break;

                default:
                    {
                        nonIdParamColumnTable += $" ,{paramName} = NULL";
                        break;
                    }
            }

            return nonIdParamColumnTable;
        }

        public static string PkIdGet(string schema, string tableName, string pkName)
        {
            var repo = new Repository();
            return repo.PrimaryKeyGet(schema, tableName, pkName);
        }

        public static string DataGet(List<string> paramNames, string schema, string tableName, string pkName, string pk)
        {
            var repo = new Repository();
            return repo.NonIdParametersDataGet(paramNames, schema, tableName, pkName, pk);
        }
    }
}
