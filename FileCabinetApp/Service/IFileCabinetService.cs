using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides functionality for interacting with records.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="record">New record.</param>
        /// <returns>Returns new record id.</returns>
        public int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Modify an existing record by id.
        /// </summary>
        /// <param name="record">New record.</param>
        public void EditRecord(FileCabinetRecord record);

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Gets count of records.
        /// </summary>
        /// <returns>Return count of records.</returns>
        public int GetStat();

        /// <summary>
        /// Find records by first name.
        /// </summary>
        /// <param name="firstName">Users first name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth);

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();
    }
}
