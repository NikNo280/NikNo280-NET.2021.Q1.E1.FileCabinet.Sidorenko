using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class representing default functions for interacting with the record model.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="record">The record that is checked for correctness.</param>
        protected override void IsValid(FileCabinetRecord record)
        {
            base.IsValid(record);
        }
    }
}
