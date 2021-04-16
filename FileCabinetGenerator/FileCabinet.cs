using FileCabinetApp;
using FileCabinetApp.Export;
using FileCabinetGenerator.RandomRecord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    public class FileCabinet
    {
        private readonly IRandomRecord randomRecord;
        private int id;
        private int recordsAmount;

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
