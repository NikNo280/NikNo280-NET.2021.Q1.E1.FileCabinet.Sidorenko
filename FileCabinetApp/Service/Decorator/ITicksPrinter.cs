using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Decorator
{
    /// <summary>
    /// Print interface.
    /// </summary>
    public interface ITicksPrinter
    {
        /// <summary>
        /// Print message to console.
        /// </summary>
        /// <param name="command">Comand name.</param>
        /// <param name="ticks">Count ticks.</param>
        public void Print(string command, long ticks);
    }
}
