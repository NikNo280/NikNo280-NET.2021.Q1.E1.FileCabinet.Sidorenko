using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Service.Decorator;
using FileCabinetApp.Service.Iterator;
using FileCabinetApp.Validation;
using FileCabinetApp.Validation.Extension;
using FileCabinetApp.Validation.InputValidation;
using NLog;
using NLog.Config;

namespace FileCabinetApp
{
    /// <summary>
    /// The main class that provides the user with functions for interacting with the system.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Nikita Sidorenko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private static bool isRunning = true;
        private static IRecordValidator recordValidator;
        private static IFileCabinetService fileCabinetService;
        private static IInputValidation inputValidation;

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

            var commandHandler = CreateCommandHandlers(Program.fileCabinetService, Program.inputValidation);
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

        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService, IInputValidation inputValidation)
        {
            if (fileCabinetService is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetService));
            }

            var helpCommandHandler = new HelpCommandHandler();
            var exitCommandHandler = new ExitCommandHandler(ExitProgram);
            var insertCommandHandler = new InsertCommandHandler(fileCabinetService);
            var deleteCommandHandler = new DeleteCommandHandler(fileCabinetService);
            var updateCommandHandler = new UpdateCommandHandler(fileCabinetService);
            var selectCommandHandler = new SelectCommandHandler(fileCabinetService, Console.WriteLine);
            var statCommandHandler = new StatCommandHandler(fileCabinetService);
            var createCommandHandler = new CreateCommandHandler(fileCabinetService, inputValidation);
            var exportCommandHandler = new ExportCommandHandler(fileCabinetService);
            var importCommandHandler = new ImportCommandHandler(fileCabinetService);
            var purgeCommandHandler = new PurgeCommandHandler(fileCabinetService);
            var missedCommandHandler = new MissedCommandHandler();

            helpCommandHandler.SetNext(exitCommandHandler);
            exitCommandHandler.SetNext(statCommandHandler);
            statCommandHandler.SetNext(insertCommandHandler);
            insertCommandHandler.SetNext(deleteCommandHandler);
            deleteCommandHandler.SetNext(updateCommandHandler);
            updateCommandHandler.SetNext(selectCommandHandler);
            selectCommandHandler.SetNext(createCommandHandler);
            createCommandHandler.SetNext(exportCommandHandler);
            exportCommandHandler.SetNext(importCommandHandler);
            importCommandHandler.SetNext(purgeCommandHandler);
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

            bool isEmpty = true;
            foreach (var record in records)
            {
                isEmpty = false;
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.Age}, {record.Salary}, {record.Gender}, " +
                    $"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
            }

            if (isEmpty)
            {
                Console.WriteLine("no records found");
            }
        }

        private static void LoadSettings(string[] args)
        {
            string[] settings = new string[] { "DEFAULT", "MEMORY", "false", "false" };
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
                else if (args[i].Equals("-s"))
                {
                    i++;
                    settings[1] = i < args.Length ? args[i].ToUpperInvariant() : "MEMORY";
                }
                else if (args[i].Equals("-use-stopwatch"))
                {
                    settings[2] = "true";
                }
                else if (args[i].Equals("-use-logger"))
                {
                    settings[3] = "true";
                }
            }

            switch (settings[0])
            {
                case "CUSTOM":
                    recordValidator = new ValidatorBuilder().CreateCustom();
                    inputValidation = new CustomInputValidation();
                    Console.WriteLine("Using custom validation rules.");
                    break;
                case "DEFAULT":
                    recordValidator = new ValidatorBuilder().CreateDefault();
                    inputValidation = new DefaultInputValidation();
                    Console.WriteLine("Using default validation rules.");
                    break;
                default:
                    recordValidator = new ValidatorBuilder().CreateDefault();
                    inputValidation = new DefaultInputValidation();
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

            if (settings[2].Equals("true"))
            {
                fileCabinetService = new ServiceMeter(fileCabinetService, new ConsoleTicksPrinter());
            }

            if (settings[3].Equals("true"))
            {
                LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
                fileCabinetService = new ServiceLogger(fileCabinetService, LogManager.GetCurrentClassLogger());
            }
        }
    }
}