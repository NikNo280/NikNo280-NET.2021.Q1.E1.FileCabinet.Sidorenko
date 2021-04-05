using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FileCabinetApp.FileCabinetService;

namespace FileCabinetApp
{
    /// <summary>
    /// Class used for writing records to files of the csv format.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="textWriter">TextWriter.</param>
        public FileCabinetRecordCsvWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        /// <summary>
        /// Writes records to the file.
        /// </summary>
        /// <param name="fileCabinetRecord">The record to be written to the file.</param>
        public void Writer(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null");
            }

            var line = $"{fileCabinetRecord.Id},{fileCabinetRecord.FirstName},{fileCabinetRecord.LastName}," +
                $"{fileCabinetRecord.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}," +
                $"{fileCabinetRecord.Age},{fileCabinetRecord.Salary},{fileCabinetRecord.Gender}";
            this.textWriter.WriteLine(line);
            this.textWriter.Flush();
        }
    }
}
