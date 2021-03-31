﻿using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Nikita Sidorenko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService;
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays quality statistics", "The 'stat' command displays quality statistics." },
            new string[] { "create", "create new record", "The 'stat' command create new record." },
            new string[] { "list", "gets a list of records", "The 'stat' command gets a list of records." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();
            fileCabinetService = new FileCabinetService();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {

            string firstName = string.Empty, lastName = string.Empty;
            DateTime dateOfBirth = default(DateTime);
            short age = -1;
            decimal salary = -1;
            char gender = default(char);
            bool result = false;
            while (firstName.Length <= 2 || firstName.Length > 60)
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
            }

            while (lastName.Length <= 2 || lastName.Length > 60)
            {
                Console.Write("Last  name: ");
                lastName = Console.ReadLine();
            }

            while ((dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1950, 1, 1)) || !result)
            {
                Console.Write("Date of birth: ");
                result = DateTime.TryParseExact(Console.ReadLine(), "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
            }

            result = false;

            while ((age < 0 || age > 110) || !result)
            {
                Console.Write("Age: ");
                result = short.TryParse(Console.ReadLine(), out age);
            }

            result = false;
            while ((salary < 0) || !result)
            {
                Console.Write("Salary: ");
                result = decimal.TryParse(Console.ReadLine(), out salary);
            }

            while (gender != 'M' && gender != 'W')
            {
                Console.Write("Gender (M/W): ");
                gender = char.ToUpper(Console.ReadLine()[0], CultureInfo.InvariantCulture);
            }

            Program.fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth, age, salary, gender);
            Console.WriteLine($"Record #{Program.fileCabinetService.GetStat()} is created.");
        }

        private static void List(string parameters)
        {
            foreach (var item in Program.fileCabinetService.GetRecords())
            {
                Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, " +
                    $"{item.Age}, {item.Salary}, {item.Gender}, " +
                    $"{item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
            }
        }
    }
}