using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class that provides the user with functions for interacting with the system.
    /// </summary>
    public static class Program
    {
        public static IRecordValidator recordValidator;
        private const string DeveloperName = "Nikita Sidorenko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService;

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

            LoadSettings(args);

            Console.WriteLine();

            var commandHandler = CreateCommandHandlers(Program.fileCabinetService);
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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(
                    new AppCommandRequest
                    {
                        Command = command,
                        Parameters = parameters,
                    });
            }
            while (isRunning);
        }

        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler(ExitProgram);
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var createCommandHandler = new CreateCommandHandler(fileCabinetService);
            var listCommandHandler = new ListCommandHandler(fileCabinetService, Print);
            var editCommandHandler = new EditCommandHandler(fileCabinetService);
            var findCommandHandler = new FindCommandHandler(fileCabinetService, Print);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var removeCommandHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            var missedCommandHandler = new MissedCommandHandler();

            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(listCommandHandler);
            listCommandHandler.SetNext(editCommandHandler);
            editCommandHandler.SetNext(findCommandHandler);
            findCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(removeCommandHandler);
            removeCommandHandler.SetNext(purgeCommandHandler);
            purgeCommandHandler.SetNext(missedCommandHandler);
            return helpCommandHandler;
        }

        private static void ExitProgram(bool isRunning)
        {
            Program.isRunning = isRunning;
        }

        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Records.</param>
        private static void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.Age}, {record.Salary}, {record.Gender}, " +
                    $"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
            }
        }

        private static void LoadSettings(string[] args)
        {
            string[] settings = new string[] { "DEFAULT", "MEMORY" };
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("--validation-rules"))
                {
                    settings[0] = args[i].Split("=")[1].ToUpperInvariant();
                }
                else if (args[i].Equals("-v"))
                {
                    i++;
                    settings[0] = i < args.Length ? args[i].ToUpperInvariant() : "DEFAULT";
                }
                else if (args[i].Contains("--storage"))
                {
                    settings[1] = args[i].Split("=")[1].ToUpperInvariant();
                }
                else if (args[i].Contains("-s"))
                {
                    i++;
                    settings[1] = i < args.Length ? args[i].ToUpperInvariant() : "MEMORY";
                }
            }

            switch (settings[0])
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

            switch (settings[1])
            {
                case "MEMORY":
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                    Console.WriteLine("Using FileCabinetMemoryService.");
                    break;
                case "FILE":
                    fileCabinetService = new FileCabinetFilesystemService(recordValidator, File.Open("cabinet-records.db", FileMode.OpenOrCreate));
                    Console.WriteLine("Using FileCabinetFilesystemService.");
                    break;
                default:
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                    Console.WriteLine("Using FileCabinetMemoryService.");
                    break;
            }
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateFirstName(string name)
        {
            if (name is null)
            {
                return new Tuple<bool, string>(false, $"{nameof(name)} is null");
            }

            if (name.Length < 2 || name.Length > 60)
            {
                return new Tuple<bool, string>(false, $"{nameof(name.Length)} is less than 2 or bigger than 60");
            }

            return new Tuple<bool, string>(true, name);
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateSecondName(string name)
        {
            if (name is null)
            {
                return new Tuple<bool, string>(false, $"{nameof(name)} is null");
            }

            if (name.Length < 1 || name.Length > 120)
            {
                return new Tuple<bool, string>(false, $"{nameof(name.Length)} is less than 1 or bigger than 120");
            }

            return new Tuple<bool, string>(true, name);
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="age">Input age.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateAge(short age)
        {
            if (age <= 18 || age > 110)
            {
                return new Tuple<bool, string>(false, $"{age} is less than 18 or bigger than 110");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="salary">Input salary.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateSalary(decimal salary)
        {
            if (salary < 1000)
            {
                return new Tuple<bool, string>(false, $"{nameof(salary)} is less than 1000");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="gender">Input gender.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateGender(char gender)
        {
            if (gender != 'M' && gender != 'W')
            {
                return new Tuple<bool, string>(false, $"{nameof(gender)} gender != 'M' && gender != 'W'");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public static Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1920, 1, 1))
            {
                return new Tuple<bool, string>(false, $"{nameof(dateOfBirth)} is less than 01-Jan-1920 or greater than current date");
            }

            return new Tuple<bool, string>(true, "ok");
        }
    }
}