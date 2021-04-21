using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a stat command hadler.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "stat", StringComparison.InvariantCultureIgnoreCase))
            {
                var recordsCount = this.FileCabinetService.GetStat();
                var recordsDeletedCount = this.FileCabinetService.GetCountDeletedRecords();
                Console.WriteLine($"{recordsCount - recordsDeletedCount} record(s). {recordsDeletedCount} deleted record(s)");
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
