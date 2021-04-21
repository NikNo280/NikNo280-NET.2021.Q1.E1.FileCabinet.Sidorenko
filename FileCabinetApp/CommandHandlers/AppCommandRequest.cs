using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a request model to the command handler.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        /// <value>
        /// Command name.
        /// </value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets command parameters.
        /// </summary>
        /// <value>
        /// Command parameters.
        /// </value>
        public string Parameters { get; set; }
    }
}
