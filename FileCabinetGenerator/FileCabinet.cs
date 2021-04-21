using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FileCabinetApp;
using FileCabinetGenerator.RandomRecord;

namespace FileCabinetGenerator
{
    /// <summary>
    /// This class provides functions for generating and writing records to a file.
    /// </summary>
    public class FileCabinet
    {
        private readonly IRandomRecord randomRecord;
        private int id;
        private int recordsAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinet"/> class.
        /// </summary>
        /// <param name="randomRecord">Random record.</param>
        /// <param name="startId">Start id.</param>
        /// <param name="countRecords">Count records.</param>
        public FileCabinet(IRandomRecord randomRecord, int startId, int countRecords)
        {
            this.randomRecord = randomRecord;
            this.id = startId;
            this.recordsAmount = countRecords;
        }

        /// <summary>
        /// Save FileCabinetRecord snapshot to csv file.
        /// </summary>
        /// <param name="streamWriter">StreamWriter.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            if (streamWriter is null)
            {
                throw new ArgumentNullException($"{nameof(streamWriter)} is null");
            }

            var line = "Id,First Name,Last Name,Date of Birth,Age,Salary,Gender";
            streamWriter.WriteLine(line);
            var cabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(streamWriter);

            for (int i = 0; i < this.recordsAmount; i++)
            {
                var record = new FileCabinetRecord()
                {
                    Id = this.id++,
                    FirstName = this.randomRecord.GetRandomName(),
                    LastName = this.randomRecord.GetRandomName(),
                    DateOfBirth = this.randomRecord.GetRandomDateOfBirth(),
                    Age = this.randomRecord.GetRandomAge(),
                    Salary = this.randomRecord.GetRandomSalary(),
                    Gender = this.randomRecord.GetRandomGender(),
                };
                cabinetRecordCsvWriter.Write(record);
            }
        }

        /// <summary>
        /// Save FileCabinetRecord snapshot xml to file.
        /// </summary>
        /// <param name="streamWriter">StreamWriter.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            if (streamWriter is null)
            {
                throw new ArgumentNullException($"{nameof(streamWriter)} is null");
            }

            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecord[]));
            List<FileCabinetRecord> fileCabinetRecords = new List<FileCabinetRecord>();
            for (int i = 0; i < this.recordsAmount; i++)
            {
                var record = new FileCabinetRecord()
                {
                    Id = this.id++,
                    FirstName = this.randomRecord.GetRandomName(),
                    LastName = this.randomRecord.GetRandomName(),
                    DateOfBirth = this.randomRecord.GetRandomDateOfBirth(),
                    Age = this.randomRecord.GetRandomAge(),
                    Salary = this.randomRecord.GetRandomSalary(),
                    Gender = this.randomRecord.GetRandomGender(),
                };
                fileCabinetRecords.Add(record);
            }

            formatter.Serialize(streamWriter, fileCabinetRecords.ToArray());
        }
    }
}
