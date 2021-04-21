using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface that provides valid function.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="parameters">Parameters that are checked for correctness.</param>
        /// <returns>Whether the parameter meets the specified criteria.</returns>
        public bool ValidateParameters(FileCabinetRecord parameters);
    }
}
