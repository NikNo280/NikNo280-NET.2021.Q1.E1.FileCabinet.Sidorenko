using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Iterator
{
    /// <summary>
    /// Iterators interface.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets next record.
        /// </summary>
        /// <returns>Record.</returns>
        public FileCabinetRecord GetNext();

        /// <summary>
        /// Checks if the records have finished.
        /// </summary>
        /// <returns>Boolean value whether records have run out.</returns>
        public bool HasMore();
    }
}
