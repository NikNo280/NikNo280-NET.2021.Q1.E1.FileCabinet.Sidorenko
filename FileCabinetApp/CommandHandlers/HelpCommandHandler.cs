using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a help command hadler.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays quality statistics", "The 'stat' command displays quality statistics." },
            new string[] { "create", "create new record", "The 'create' command create new record." },
            new string[] { "list", "gets a list of records", "The 'list' command gets a list of records." },
            new string[] { "edit", "modify a data of an existing record.", "The 'edit' command modify a data of an existing record.." },
            new string[] { "find", "finds records.", "The 'find' command finds records." },
            new string[] { "export", "exports service data to file", "The 'export' command exports service data to file" },
            new string[] { "import", "imports data from a file into a service", "The 'import' command imports data from a file into a service" },
            new string[] { "remove", "removes record by id", "The 'remove' removes record by id" },
            new string[] { "purge", "defragments the data file", "The 'purge' defragments the data file" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public HelpCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Command handler.
        /// </summary>
        /// <param name="appCommandRequest">Request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException($"{nameof(appCommandRequest)} is null");
            }

            if (string.Equals(appCommandRequest.Command, "help", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(appCommandRequest.Parameters))
                {
                    var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], appCommandRequest.Parameters, StringComparison.InvariantCultureIgnoreCase));
                    if (index >= 0)
                    {
                        Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
                    }
                    else
                    {
                        Console.WriteLine($"There is no explanation for '{appCommandRequest.Parameters}' command.");
                    }
                }
                else
                {
                    Console.WriteLine("Available commands:");

                    foreach (var helpMessage in helpMessages)
                    {
                        Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                    }
                }

                Console.WriteLine();
            }
            else
            {
                if (this.NextCommandHandler != null)
                {
                    this.NextCommandHandler.Handle(appCommandRequest);
                }
                else
                {
                    throw new ArgumentException("Invalid command.");
                }
            }
        }
    }
}
