using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class of command handlers.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// File cabinet service.
        /// </summary>
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Following command handler.
        /// </summary>
        private ICommandHandler nextCommandHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        protected CommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
        }

        /// <summary>
        /// Gets next command handler.
        /// </summary>
        /// <value>
        /// Next command handler.
        /// </value>
        protected ICommandHandler NextCommandHandler
        {
            get
            {
                return this.nextCommandHandler;
            }
        }

        /// <summary>
        /// Gets file cabinet service.
        /// </summary>
        /// <value>
        /// File cabinet service.
        /// </value>
        protected IFileCabinetService FileCabinetService
        {
            get
            {
                return this.fileCabinetService;
            }
        }

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="appCommandRequest">Request.</param>
        public abstract void Handle(AppCommandRequest appCommandRequest);

        /// <summary>
        /// This method provides the ability to specify the following handler.
        /// </summary>
        /// <param name="commandHandler">Following command handler.</param>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextCommandHandler = commandHandler;
        }
    }
}
