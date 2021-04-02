using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Class representing the model record.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>
        /// User id.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        /// <value>
        /// User first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user last name.
        /// </summary>
        /// <value>
        /// User last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user date of birth.
        /// </summary>
        /// <value>
        /// User date of birth.
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user age.
        /// </summary>
        /// <value>
        /// User age.
        /// </value>
        public short Age { get; set; }

        /// <summary>
        /// Gets or sets user salary.
        /// </summary>
        /// <value>
        /// User salary.
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets user gender.
        /// </summary>
        /// <value>
        /// User gender.
        /// </value>
        public char Gender { get; set; }
    }
}
