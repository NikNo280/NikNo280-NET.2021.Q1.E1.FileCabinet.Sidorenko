using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class that provides the user with functions for interacting with the system.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikita Sidorenko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetMemoryService fileCabinetService;
        private static bool isRunning = true;
        private static IRecordValidator recordValidator;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays quality statistics", "The 'stat' command displays quality statistics." },
            new string[] { "create", "create new record", "The 'stat' command create new record." },
            new string[] { "list", "gets a list of records", "The 'stat' command gets a list of records." },
            new string[] { "edit", "modify a data of an existing record.", "The 'stat' command modify a data of an existing record.." },
            new string[] { "find", "finds records.", "The 'stat' command finds records." },
            new string[] { "export", "exports service data to file", "The 'stat' command export exports service data to file" },
        };

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">Programs arguments.</param>
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            if (args.Length == 0)
            {
                recordValidator = new DefaultValidator();
                Console.WriteLine("Using default validation rules.");
            }
            else if (args[0].Contains("--validation-rules"))
            {
                switch (args[0].Split("=")[1].ToUpperInvariant())
                {
                    case "CUSTOM":
                        recordValidator = new CustomValidator();
                        Console.WriteLine("Using custom validation rules.");
                        break;
                    case "DEFAULT":
                        recordValidator = new DefaultValidator();
                        Console.WriteLine("Using default validation rules.");
                        break;
                    default:
                        recordValidator = new DefaultValidator();
                        Console.WriteLine("Using default validation rules.");
                        break;
                }
            }
            else if (args[0].Equals("-v"))
            {
                switch (args[1].ToUpperInvariant())
                {
                    case "CUSTOM":
                        recordValidator = new CustomValidator();
                        Console.WriteLine("Using custom validation rules.");
                        break;
                    case "DEFAULT":
                        recordValidator = new DefaultValidator();
                        Console.WriteLine("Using default validation rules.");
                        break;
                    default:
                        recordValidator = new DefaultValidator();
                        Console.WriteLine("Using default validation rules.");
                        break;
                }
            }
            else
            {
                recordValidator = new DefaultValidator();
                Console.WriteLine("Using default validation rules.");
            }

            fileCabinetService = new FileCabinetMemoryService(recordValidator);

            Console.WriteLine();

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
            Console.Write("First name: ");
            var firstName = TypeConverter.ReadInput(TypeConverter.StringConverter, recordValidator.NameValidator);
            Console.Write("Last  name: ");
            var lastName = TypeConverter.ReadInput(TypeConverter.StringConverter, recordValidator.NameValidator);
            Console.Write("Date of birth: ");
            var dateOfBirth = TypeConverter.ReadInput(TypeConverter.DateTimeConverter, recordValidator.DateOfBirthValidator);
            Console.Write("Age: ");
            var age = TypeConverter.ReadInput(TypeConverter.ShortConverter, recordValidator.AgeValidator);
            Console.Write("Salary: ");
            var salary = TypeConverter.ReadInput(TypeConverter.DecimalConverter, recordValidator.SalaryValidator);
            Console.Write("Gender (M/W): ");
            var gender = TypeConverter.ReadInput(TypeConverter.CharConverter, recordValidator.GenderValidator);
            var record = new FileCabinetRecord
            {
                Id = Program.fileCabinetService.GetStat() + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Age = age,
                Salary = salary,
                Gender = gender,
            };
            Program.fileCabinetService.CreateRecord(record);
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
            bool result;
            int id;
            result = int.TryParse(parameters, out id);
            if (!result)
            {
                Console.WriteLine("id is not a number");
                return;
            }

            Console.Write("First name: ");
            var firstName = TypeConverter.ReadInput(TypeConverter.StringConverter, recordValidator.NameValidator);
            Console.Write("Last  name: ");
            var lastName = TypeConverter.ReadInput(TypeConverter.StringConverter, recordValidator.NameValidator);
            Console.Write("Date of birth: ");
            var dateOfBirth = TypeConverter.ReadInput(TypeConverter.DateTimeConverter, recordValidator.DateOfBirthValidator);
            Console.Write("Age: ");
            var age = TypeConverter.ReadInput(TypeConverter.ShortConverter, recordValidator.AgeValidator);
            Console.Write("Salary: ");
            var salary = TypeConverter.ReadInput(TypeConverter.DecimalConverter, recordValidator.SalaryValidator);
            Console.Write("Gender (M/W): ");
            var gender = TypeConverter.ReadInput(TypeConverter.CharConverter, recordValidator.GenderValidator);
            try
            {
                var editRecord = new FileCabinetRecord
                {
                    Id = id,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Age = age,
                    Salary = salary,
                    Gender = gender,
                };
                Program.fileCabinetService.EditRecord(editRecord);
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
            ReadOnlyCollection<FileCabinetRecord> records = parametersSplit[0].ToUpperInvariant() switch
            {
                "FIRSTNAME" => Program.fileCabinetService.FindByFirstName(parametersSplit[1]),
                "LASTNAME" => Program.fileCabinetService.FindByLastName(parametersSplit[1]),
                "DATEOFBIRTH" => Program.fileCabinetService.FindByDateOfBirth(parametersSplit[1]),
                _ => new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>())
            };
            if (records.Count == 0)
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

        private static void Export(string parameters)
        {
            string[] data = parameters.Split(' ');
            if (data.Length != 2)
            {
                Console.WriteLine("Incorrect parameters");
                return;
            }

            try
            {
                var snapshot = Program.fileCabinetService.MakeSnapshot();
                if (File.Exists(data[1]))
                {
                    Console.Write($"File is exist - rewrite {data[1]}? [Y/n] ");
                    string rewriting = Console.ReadLine();
                    if (rewriting.ToUpperInvariant() != "Y")
                    {
                        return;
                    }
                }

                using (var streamW = new StreamWriter(data[1]))
                {
                    switch (data[0].ToUpperInvariant())
                    {
                        case "CSV":
                            snapshot.SaveToCsv(streamW);
                            Console.WriteLine($"All records are exported to file {data[1]}.");
                            break;
                        case "XML":
                            snapshot.SaveToXml(streamW);
                            Console.WriteLine($"All records are exported to file {data[1]}.");
                            break;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}