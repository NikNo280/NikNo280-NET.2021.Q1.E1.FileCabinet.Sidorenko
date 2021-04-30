using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Decorator
{
    /// <summary>
    /// This class provides print function.
    /// </summary>
    public class ConsoleTicksPrinter : ITicksPrinter
    {
        /// <summary>
        /// Print message to console.
        /// </summary>
        /// <param name="command">Comand name.</param>
        /// <param name="ticks">Count ticks.</param>
        public void Print(string command, long ticks)
        {
            Console.WriteLine($"{command} method execution duration is {ticks} ticks.");
        }
    }
}
