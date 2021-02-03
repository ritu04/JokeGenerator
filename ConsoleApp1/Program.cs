using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static string[] results = new string[50];
        static char key;
        static Tuple<string, string> names;
        static ConsolePrinter printer = new ConsolePrinter();
        static readonly string jokesURL = "https://api.chucknorris.io";

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("jokeGeneratorLog.log").CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();

            serviceProvider.GetService<JsonFeed>();

            logger.LogInformation("Log in Program.cs");

            try
            {
                printer.Value("Press ? to get instructions.").ToString();
                if (Console.ReadLine() == "?")
                {
                    while (true)
                    {
                        printer.Value("Press c to get categories").ToString();
                        printer.Value("Or").ToString();
                        printer.Value("Press r to get random jokes").ToString();
                        GetEnteredKey(Console.ReadKey());
                        Console.WriteLine("");

                        if (key == 'c')
                        {
                            getCategories();
                            PrintResults();
                        }
                        if (key == 'r')
                        {
                            printer.Value("Want to use a random name? y/n").ToString();
                            GetEnteredKey(Console.ReadKey());

                            if (key == 'y')
                                GetNames();

                            key = '\0';

                            Console.WriteLine();

                            printer.Value("Want to specify a category? y/n").ToString();
                            GetEnteredKey(Console.ReadKey());

                            if (key == 'y')
                            {
                                printer.Value("How many jokes do you want? (1-9)").ToString();
                                int n;
                                if (Int32.TryParse(Console.ReadLine(), out n) && n < 10)
                                {
                                    printer.Value("Enter a category;").ToString();
                                    GetRandomJokes(Console.ReadLine(), n);
                                    PrintResults();
                                }
                                else
                                    printer.Value("Incorrect response. Try again!").ToString();
                            }
                            else
                            {
                                printer.Value("How many jokes do you want? (1-9)").ToString();
                                int n;
                                if (Int32.TryParse(Console.ReadLine(), out n) && n < 10)
                                {
                                    GetRandomJokes(null, n);
                                    PrintResults();
                                }
                                else
                                    printer.Value("Incorrect response. Try again!").ToString();
                            }
                        }
                        else
                            printer.Value("Incorrect response. Try again!").ToString();
                        names = null;
                    }
                }
                else
                    printer.Value("Incorrect response. Try again!").ToString();
            }
            catch
            {
                printer.Value("Incorrect response. Try again!").ToString();
            }
        }

        private static void PrintResults()
        {
            printer.Value("[" + string.Join(",", results) + "]").ToString();
        }

        private static void GetEnteredKey(ConsoleKeyInfo consoleKeyInfo)
        {
            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.C:
                    key = 'c';
                    break;
                case ConsoleKey.D0:
                    key = '0';
                    break;
                case ConsoleKey.D1:
                    key = '1';
                    break;
                case ConsoleKey.D3:
                    key = '3';
                    break;
                case ConsoleKey.D4:
                    key = '4';
                    break;
                case ConsoleKey.D5:
                    key = '5';
                    break;
                case ConsoleKey.D6:
                    key = '6';
                    break;
                case ConsoleKey.D7:
                    key = '7';
                    break;
                case ConsoleKey.D8:
                    key = '8';
                    break;
                case ConsoleKey.D9:
                    key = '9';
                    break;
                case ConsoleKey.R:
                    key = 'r';
                    break;
                case ConsoleKey.Y:
                    key = 'y';
                    break;
            }
        }

        private static void GetRandomJokes(string category, int number)
        {
            new JsonFeed(jokesURL, number);
            results = JsonFeed.GetRandomJokes(names?.Item1, names?.Item2, category);
        }

        private static void getCategories()
        {
            new JsonFeed(jokesURL, 0);
            results = JsonFeed.GetCategories();
        }

        private static void GetNames()
        {
            new JsonFeed("https://www.names.privserv.com/api/", 0);
            dynamic result = JsonFeed.Getnames();
            names = Tuple.Create(result.name.ToString(), result.surname.ToString());
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog())
                    .AddTransient<JsonFeed>();
        }
    }
}
