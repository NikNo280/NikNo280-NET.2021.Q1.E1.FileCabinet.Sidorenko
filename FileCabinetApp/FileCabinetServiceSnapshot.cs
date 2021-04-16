using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using FileCabinetApp.Export;
using FileCabinetApp.Import;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetRecord snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;
        private int recordsImportCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Array of records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets records.
        /// </summary>
        /// <value>
        /// ReadOnlyCollection of records.
        /// </value>
        public ReadOnlyCollection<FileCabinetRecord> Records
        {
            get
            {
                return new ReadOnlyCollection<FileCabinetRecord>(this.records);
            }
        }

        /// <summary>
        /// Gets the number of import records.
        /// </summary>
        /// <value>
        /// The number of import records.
        /// </value>
        public int RecordsImportCount
        {
            get
            {
                return this.recordsImportCount;
            }
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

            foreach (var item in this.records)
            {
                cabinetRecordCsvWriter.Write(item);
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

            XmlWriter xmlWriter = XmlWriter.Create(streamWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            var cabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            foreach (var item in this.records)
            {
                cabinetRecordXmlWriter.Write(item);
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }

        /// <summary>
        /// Reads data from an Csv file.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public void LoadFromCsv(StreamReader streamReader)
        {
            var csvRecordsReader = new FileCabinetRecordCsvReader(streamReader);
            var records = new List<FileCabinetRecord>();
            foreach (var record in csvRecordsReader.ReadAll())
            {
                records.Add(record);
            }

            this.recordsImportCount = records.Count;
            this.records = records.ToArray();
        }

        /// <summary>
        /// Reads data from an XML file.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public void LoadFromXml(StreamReader streamReader)
        {
            var xmlRecordsReader = new FileCabinetRecordXmlReader(streamReader);
            var records = new List<FileCabinetRecord>();
            foreach (var record in xmlRecordsReader.ReadAll())
            {
                records.Add(record);
            }

            this.recordsImportCount = records.Count;
            this.records = records.ToArray();
        }
    }
}
