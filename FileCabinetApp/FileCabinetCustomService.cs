using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class representing custom functions for interacting with the record model.
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="record">The record that is checked for correctness.</param>
        protected override void IsValid(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (record.FirstName is null)
            {
                throw new ArgumentNullException($"{nameof(record.FirstName)} is null");
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException($"{nameof(record.LastName)} is null");
            }

            if (record.FirstName.Length <= 1 || record.FirstName.Length > 120)
            {
                throw new ArgumentException($"{nameof(record.FirstName.Length)} is less than 2 or bigger than 60");
            }

            if (record.LastName.Length <= 1 || record.LastName.Length > 120)
            {
                throw new ArgumentException($"{nameof(record.LastName.Length)} is less than 2 or bigger than 60");
            }

            if (record.FirstName.Equals(new string(' ', record.FirstName.Length)))
            {
                throw new ArgumentException($"{nameof(record.FirstName)} consists only of spaces");
            }

            if (record.LastName.Equals(new string(' ', record.LastName.Length)))
            {
                throw new ArgumentException($"{nameof(record.LastName)} consists only of spaces");
            }

            if (record.Age < 18 || record.Age > 110)
            {
                throw new ArgumentException($"{nameof(record.Age)} is less than zero or bigger than 110");
            }

            if (record.Salary < 1000)
            {
                throw new ArgumentException($"{nameof(record.Salary)} is less than zero");
            }

            if (record.DateOfBirth >= DateTime.Now || record.DateOfBirth <= new DateTime(1950, 1, 1))
            {
                throw new ArgumentException($"{nameof(record.DateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            if (record.Gender != 'M' && record.Gender != 'W')
            {
                throw new ArgumentException($"{nameof(record.Gender)} gender != 'M' && gender != 'W'");
            }
        }
    }
}
