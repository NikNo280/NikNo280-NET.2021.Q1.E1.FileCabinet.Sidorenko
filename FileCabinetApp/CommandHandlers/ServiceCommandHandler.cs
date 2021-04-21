using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class of command handlers with fileCabinetService.
    /// </summary>
    public abstract class ServiceCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// File cabinet service.
        /// </summary>
        private IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        protected ServiceCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
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
    }
}
