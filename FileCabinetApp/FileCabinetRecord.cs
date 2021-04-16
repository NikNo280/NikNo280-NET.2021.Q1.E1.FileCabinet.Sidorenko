using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Class representing the model record.
    /// </summary>
    [XmlRoot("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>
        /// User id.
        /// </value>
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        /// <value>
        /// User first name.
        /// </value>
        [XmlElement(ElementName = "first")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user last name.
        /// </summary>
        /// <value>
        /// User last name.
        /// </value>
        [XmlElement(ElementName = "last")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user date of birth.
        /// </summary>
        /// <value>
        /// User date of birth.
        /// </value>
        [XmlElement(ElementName = "dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user age.
        /// </summary>
        /// <value>
        /// User age.
        /// </value>
        [XmlElement(ElementName = "age")]
        public short Age { get; set; }

        /// <summary>
        /// Gets or sets user salary.
        /// </summary>
        /// <value>
        /// User salary.
        /// </value>
        [XmlElement(ElementName = "salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets user gender.
        /// </summary>
        /// <value>
        /// User gender.
        /// </value>
        [XmlElement(ElementName = "gender")]
        public char Gender { get; set; }
    }
}
