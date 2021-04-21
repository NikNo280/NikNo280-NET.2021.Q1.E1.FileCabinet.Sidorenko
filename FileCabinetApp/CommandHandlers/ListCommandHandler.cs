using System;
using FileCabinetApp.CommandHandlers.Interface;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a list command hadler.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandler
    {
        private IRecordPrinter recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter recordPrinter)
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

            if (string.Equals(appCommandRequest.Command, "list", StringComparison.InvariantCultureIgnoreCase))
            {
                this.recordPrinter.Print(this.FileCabinetService.GetRecords());
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
