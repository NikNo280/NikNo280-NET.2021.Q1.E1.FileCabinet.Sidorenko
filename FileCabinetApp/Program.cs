using System;
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
        private static bool isRunning = true;
        private const string DeveloperName = "Nikita Sidorenko";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
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
            var listCommandHandler = new ListCommandHandler(fileCabinetService);
            var editCommandHandler = new EditCommandHandler(fileCabinetService);
            var findCommandHandler = new FindCommandHandler(fileCabinetService);
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
    }
}