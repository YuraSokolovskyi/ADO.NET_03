using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using CustomMenu;
using System.Diagnostics;
using System.Numerics;

namespace homework_01
{
    class Program
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        private static string ProviderName = ConfigurationManager.ConnectionStrings["DefaultConnection"].ProviderName;

        private static string CountriesQuery =
            "SELECT Countries.Id, Countries.Name AS Name, Area, Population, R.Name AS Region FROM Countries JOIN dbo.Regions R on Countries.RegionId = R.Id";
        private static string CitiesQuery =
            "SELECT Cities.Id, Cities.Name AS Name, IsCapital, Cities.Population, C.Name AS Country FROM Cities JOIN dbo.Countries C on Cities.CountryId = C.Id";
        private static string RegionsQuery =
            "SELECT Id, Name FROM Regions";

        private static Dictionary<string, List<string>> Countries = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> Cities = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> Regions = new Dictionary<string, List<string>>();
        
        private static async Task<Dictionary<string, List<string>>> ReadDataAsync(DbProviderFactory factory, List<string> columns, string query)
        {
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            foreach (string column in columns) data[column] = new List<string>();
            
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;
                await connection.OpenAsync();

                using (DbCommand command = factory.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = query;
                
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            foreach (string column in columns) data[column].Add(reader[column].ToString().Trim());
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return data;
        }
        
        static async Task Main(string[] args)
        {
            Menu menu = new Menu();
            // set up header
            menu.addHeaderRow(
            new List<string>(){
                "Show all countries: T",
                "Show all cities: C",
                "Show all country names: N",
                "Show all capitals: X",
                "Show all capitals with population more than 5 millions: F",
                "Show all countries in Europe: E",
                "Show all countries with bigger area: M",
                "Show all capitals with 'a' and 'p' in name: 1",
                "Show all capitals which names start with 'k': 2",
                "Show all countries with area in range: 3",
                "Show all countries with bigger population: 4",
                "Show TOP 5 countries by area: A",
                "Show country with max area: 5",
                "Show capital with max population: 6",
                "Show the smallest country in Europe: 7",
                "Show an average countries area in Europe: 8",
                "Show TOP 3 cities by population for country: 9",
                "Show region with max number of countries: L",
                "Show number of countries in each region: P",
            });
            menu.setHeaderDivider("\n");
            menu.setHeaderEndString("\n\n");

            DbProviderFactories.RegisterFactory(ProviderName, SqlClientFactory.Instance);
            DbProviderFactory factory = DbProviderFactories.GetFactory(ProviderName);

            Countries = await ReadDataAsync(factory, new List<string>()
            {
                "Id",
                "Name",
                "Area",
                "Population",
                "Region",
            }, CountriesQuery);
            
            Cities = await ReadDataAsync(factory, new List<string>()
            {
                "Id",
                "Name",
                "IsCapital",
                "Population",
                "Country",
            }, CitiesQuery);
            
            Regions = await ReadDataAsync(factory, new List<string>()
            {
                "Id",
                "Name"
            }, RegionsQuery);
           
           // add options for main loop
           menu.addMainLoopOption(new Dictionary<ConsoleKey, Menu.MainLoopOptionDelegate>()
           {
               { ConsoleKey.T, () => menu.printTableColumn(Countries, showRowNumber:true) },
               { ConsoleKey.C, () => menu.printTableColumn(Cities, showRowNumber:true) },
               { ConsoleKey.N, () =>
               {
                   menu.printTableColumn(
                       new List<string>(){"Name"}, 
                       new List<List<string>> {Countries["Name"]}, 
                       showRowNumber:true, autoGenerateId:true);
               }},
               { ConsoleKey.X, () =>
               {
                   var res = Cities["IsCapital"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "1")
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.F, () =>
               {
                   var res = Cities["IsCapital"]
                       .Select((item, index) => new {Item=item, Index=index})
                       .Where(item => item.Item == "1")
                       .Select(item => item.Index)
                       .Where(index =>
                           long.Parse(Cities["Population"][index]) >= 5000000)
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.E, () =>
               {
                   var res = Countries["Region"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "Europe")
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.M, () =>
               {
                   Console.Write("Enter area: ");
                   long area = long.Parse(Console.ReadLine());
                   var res = Countries["Area"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => long.Parse(item.Item) > area)
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D1, () =>
               {
                   var res = Cities["IsCapital"]
                       .Select((item, index) => new {Item=item, Index=index})
                       .Where(item => item.Item == "1")
                       .Select(item => item.Index)
                       .Where(index =>
                           Cities["Name"][index].ToLower().Contains('a') &&
                           Cities["Name"][index].ToLower().Contains('p'))
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D2, () =>
               {
                   var res = Cities["IsCapital"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "1")
                       .Select(item => item.Index)
                       .Where(item => Cities["Name"][item].ToLower().StartsWith('k'))
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D3, () =>
               {
                   Console.Write("Enter min area: ");
                   long minArea = long.Parse(Console.ReadLine());
                   Console.Write("Enter max area: ");
                   long maxArea = long.Parse(Console.ReadLine());

                   var res = Countries["Area"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => long.Parse(item.Item) > minArea && long.Parse(item.Item) < maxArea)
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D4, () =>
               {
                   Console.Write("Enter population: ");
                   long population = long.Parse(Console.ReadLine());

                   var res = Countries["Population"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => long.Parse(item.Item) > population)
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.A, () =>
               {
                   var res = Countries["Area"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .OrderByDescending(item => long.Parse(item.Item))
                       .Take(5)
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D5, () =>
               {
                   var res = Countries["Area"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .OrderByDescending(item => long.Parse(item.Item))
                       .Take(1)
                       .Select(item => item.Index)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D6, () =>
               {
                   var res = Cities["IsCapital"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "1")
                       .Select(item => item.Index)
                       .OrderByDescending(index => long.Parse(Cities["Population"][index]))
                       .Take(1)
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D7, () =>
               {
                   var res = Countries["Region"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "Europe")
                       .Select(item => item.Index)
                       .OrderBy(index => long.Parse(Countries["Area"][index]))
                       .Take(1)
                       .ToList();
                   menu.printTableColumn(Countries, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.D8, () =>
               {
                   var res = Countries["Region"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == "Europe")
                       .Select(item => item.Index)
                       .Sum(index => long.Parse(Countries["Area"][index])) / Countries["Area"].Count;
                   menu.printSingleAnswer("An average countries area in Europe: ", res.ToString());
               }},
               { ConsoleKey.D9, () =>
               {
                   Console.Write("Enter country: ");
                   string country = Console.ReadLine();
                   
                   var res = Cities["Country"]
                       .Select((item, index) => new { Item = item, Index = index })
                       .Where(item => item.Item == country)
                       .Select(item => item.Index)
                       .OrderByDescending(index => Cities["Population"][index])
                       .Take(3)
                       .ToList();
                   menu.printTableColumn(Cities, showRowNumber:true, rowsToShow:res);
               }},
               { ConsoleKey.L, () =>
               {
                   var res = Regions["Name"]
                       .OrderByDescending(item => Countries["Region"].Count(x => x == item))
                       .Take(1)
                       .ToList()[0]; 
                   menu.printSingleAnswer("Region with max number of countries: ", res);
               }},
               { ConsoleKey.P, () =>
               {
                   foreach (string region in Regions["Name"]) 
                       menu.printSingleAnswer($"Countries in {region}: ", 
                           Countries["Region"].Count(item => item == region).ToString());
               }},
           });
           menu.startMainLoop();
        }
    }
}