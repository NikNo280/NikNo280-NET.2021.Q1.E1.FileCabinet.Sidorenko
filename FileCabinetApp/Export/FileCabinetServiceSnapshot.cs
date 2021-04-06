using System;
using System.IO;
using System.Xml;
using FileCabinetApp.Export;

namespace FileCabinetApp
{
    /// <summary>
    /// FileCabinetRecord snapshot.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Array of records.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
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
                cabinetRecordCsvWriter.Writer(item);
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
                cabinetRecordXmlWriter.Writer(item);
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
        }
    }
}
