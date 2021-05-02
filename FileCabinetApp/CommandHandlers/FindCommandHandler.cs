using System;
using System.Collections.Generic;
using FileCabinetApp.Service.Iterator;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a find command hadler.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandler
    {
        private Action<IEnumerable<FileCabinetRecord>> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        /// <param name="recordPrinter">Record printer.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> recordPrinter)
            : base(fileCabinetService)
        {
            this.recordPrinter = recordPrinter;
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

            if (string.Equals(appCommandRequest.Command, "find", StringComparison.InvariantCultureIgnoreCase))
            {
                var parametersSplit = appCommandRequest.Parameters.Split(" ");
                if (parametersSplit.Length != 2 || string.IsNullOrWhiteSpace(parametersSplit[0]) || string.IsNullOrWhiteSpace(parametersSplit[1]))
                {
                    Console.WriteLine("Error input");
                    return;
                }

                IEnumerable<FileCabinetRecord> recordsIterator = parametersSplit[0].ToUpperInvariant() switch
                {
                    "FIRSTNAME" => this.FileCabinetService.FindByFirstName(parametersSplit[1]),
                    "LASTNAME" => this.FileCabinetService.FindByLastName(parametersSplit[1]),
                    "DATEOFBIRTH" => this.FileCabinetService.FindByDateOfBirth(parametersSplit[1]),
                    _ => null
                };

                if (recordsIterator is null)
                {
                    Console.WriteLine("no matching parameter found");
                }
                else
                {
                    this.recordPrinter(recordsIterator);
                }
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
