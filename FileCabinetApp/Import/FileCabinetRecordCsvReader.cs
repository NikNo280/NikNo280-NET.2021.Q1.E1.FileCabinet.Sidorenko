using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Import
{
    /// <summary>
    /// This class provides functions to read data from a csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public FileCabinetRecordCsvReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Gets all records from csv file.
        /// </summary>
        /// <returns>IList of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var records = new List<FileCabinetRecord>();
            this.reader.ReadLine();
            string line;
            string[] fields;
            while ((line = this.reader.ReadLine()) != null)
            {
                fields = line.Split(',');
                var record = new FileCabinetRecord
                {
                    Id = int.Parse(fields[0], NumberStyles.Integer, CultureInfo.InvariantCulture),
                    FirstName = fields[1],
                    LastName = fields[2],
                    DateOfBirth = DateTime.ParseExact(fields[3], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                    Age = short.Parse(fields[4], CultureInfo.InvariantCulture),
                    Salary = (decimal)double.Parse(fields[5], CultureInfo.InvariantCulture),
                    Gender = fields[6][0],
                };
                records.Add(record);
            }

            return records;
        }
    }
}
