using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Сommand handler interface.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// This method provides the ability to specify the following handler.
        /// </summary>
        /// <param name="commandHandler">Following command handler.</param>
        public void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Handle.
        /// </summary>
        /// <param name="appCommandRequest">Request.</param>
        public void Handle(AppCommandRequest appCommandRequest);
    }
}
