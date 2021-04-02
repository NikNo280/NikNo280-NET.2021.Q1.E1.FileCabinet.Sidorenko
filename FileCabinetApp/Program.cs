using System;
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
        private const int MaxNameLength = 60;
        private const int MinNmaeLength = 2;
        private const int MaxAge = 110;
        private const int MinAge = 0;
        private const int MinSalary = 0;
        private static FileCabinetService fileCabinetService;
        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays quality statistics", "The 'stat' command displays quality statistics." },
            new string[] { "create", "create new record", "The 'stat' command create new record." },
            new string[] { "list", "gets a list of records", "The 'stat' command gets a list of records." },
            new string[] { "edit", "modify a data of an existing record.", "The 'stat' command modify a data of an existing record.." },
            new string[] { "find", "finds records by firstName.", "The 'stat' command finds records by firstName." },
        };

        public static void Main()
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
            while (firstName.Length <= MinNmaeLength || firstName.Length > MaxNameLength)
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
            }

            while (lastName.Length <= MinNmaeLength || lastName.Length > MaxNameLength)
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

            while ((age < MinAge || age > MaxAge) || !result)
            {
                Console.Write("Age: ");
                result = short.TryParse(Console.ReadLine(), out age);
            }

            result = false;
            while ((salary < MinSalary) || !result)
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

        private static void Edit(string parameters)
        {
            string firstName = string.Empty, lastName = string.Empty;
            DateTime dateOfBirth = default(DateTime);
            short age = -1;
            decimal salary = -1;
            char gender = default(char);
            bool result = false;
            int id;
            result = int.TryParse(parameters, out id);
            if (!result)
            {
                Console.WriteLine("id is not a number");
                return;
            }

            result = false;
            while (firstName.Length <= MinNmaeLength || firstName.Length > MaxNameLength)
            {
                Console.Write("First name: ");
                firstName = Console.ReadLine();
            }

            while (lastName.Length <= MinNmaeLength || lastName.Length > MaxNameLength)
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

            while ((age < MinAge || age > MaxAge) || !result)
            {
                Console.Write("Age: ");
                result = short.TryParse(Console.ReadLine(), out age);
            }

            result = false;
            while ((salary < MinSalary) || !result)
            {
                Console.Write("Salary: ");
                result = decimal.TryParse(Console.ReadLine(), out salary);
            }

            while (gender != 'M' && gender != 'W')
            {
                Console.Write("Gender (M/W): ");
                gender = char.ToUpper(Console.ReadLine()[0], CultureInfo.InvariantCulture);
            }

            try
            {
                Program.fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth, age, salary, gender);
                Console.WriteLine($"Record #{id} is updated.");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Find(string parameters)
        {
            var parametersSplit = parameters.Split(" ");
            FileCabinetRecord[] records = parametersSplit[0].ToUpperInvariant() switch
            {
                "FIRSTNAME" => Program.fileCabinetService.FindByFirstName(parametersSplit[1]),
                "LASTNAME" => Program.fileCabinetService.FindByLastName(parametersSplit[1]),
                "DATEOFBIRTH" => Program.fileCabinetService.FindByDateOfBirth(parametersSplit[1]),
                _ => Array.Empty<FileCabinetRecord>()
            };
            if (records.Length == 0)
            {
                Console.WriteLine("no records found");
            }
            else
            {
                foreach (var item in records)
                {
                    Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, " +
                        $"{item.Age}, {item.Salary}, {item.Gender}, " +
                        $"{item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
                }
            }
        }
    }
}