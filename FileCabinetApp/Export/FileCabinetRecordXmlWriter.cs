using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCabinetApp.Export
{
    /// <summary>
    /// Class used for writing records to files of the xml format.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="xmlWriter">XmlWriter.</param>
        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter)
        {
            this.xmlWriter = xmlWriter;
        }

        /// <summary>
        /// Writes records to the file.
        /// </summary>
        /// <param name="fileCabinetRecord">The record to be written to the file.</param>
        public void Write(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null");
            }

            this.xmlWriter.WriteStartElement("record");
            this.xmlWriter.WriteAttributeString("id", fileCabinetRecord.Id.ToString(CultureInfo.InvariantCulture));

            this.xmlWriter.WriteStartElement("name");
            this.xmlWriter.WriteAttributeString("last", fileCabinetRecord.LastName);
            this.xmlWriter.WriteAttributeString("first", fileCabinetRecord.FirstName);
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement("dateOfBirth");
            this.xmlWriter.WriteString(fileCabinetRecord.DateOfBirth.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement("Age");
            this.xmlWriter.WriteString(fileCabinetRecord.Age.ToString(CultureInfo.InvariantCulture));
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement("Salary");
            this.xmlWriter.WriteString(fileCabinetRecord.Salary.ToString(CultureInfo.InvariantCulture));
            this.xmlWriter.WriteEndElement();

            this.xmlWriter.WriteStartElement("Gender");
            this.xmlWriter.WriteString(fileCabinetRecord.Gender.ToString());
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.Flush();
        }
    }
}
