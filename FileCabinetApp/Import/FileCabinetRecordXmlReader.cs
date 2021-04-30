using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp.Import
{
    /// <summary>
    /// This class provides functions to read data from a xml file.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">StreamReader.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader)
        {
            this.reader = XmlReader.Create(streamReader);
        }

        /// <summary>
        /// Gets all records from xml file.
        /// </summary>
        /// <returns>IList of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FileCabinetRecord[]));
            if (formatter.CanDeserialize(this.reader))
            {
                return (FileCabinetRecord[])formatter.Deserialize(this.reader);
            }
            else
            {
                Console.WriteLine("Failed to read data");
                return Array.Empty<FileCabinetRecord>();
            }
        }
    }
}
