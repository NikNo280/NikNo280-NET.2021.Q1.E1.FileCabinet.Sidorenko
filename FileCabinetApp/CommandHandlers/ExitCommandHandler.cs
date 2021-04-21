using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a exit command hadler.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public ExitCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;
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
