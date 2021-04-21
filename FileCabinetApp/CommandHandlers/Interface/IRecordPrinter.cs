using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers.Interface
{
    /// <summary>
    /// Print records.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Print(IEnumerable<FileCabinetRecord> records);
    }
}
