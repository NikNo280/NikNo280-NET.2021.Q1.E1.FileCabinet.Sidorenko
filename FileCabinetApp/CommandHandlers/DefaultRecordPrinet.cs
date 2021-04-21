using System;
using System.Collections.Generic;
using System.Globalization;
using FileCabinetApp.CommandHandlers.Interface;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Print records.
    /// </summary>
    public class DefaultRecordPrinet : IRecordPrinter
    {
        /// <summary>
        /// Print records.
        /// </summary>
        /// <param name="records">Records.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.Age}, {record.Salary}, {record.Gender}, " +
                    $"{record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
            }
        }
    }
}
