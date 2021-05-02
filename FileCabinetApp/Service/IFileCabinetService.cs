using System.Collections.ObjectModel;
using FileCabinetApp.Service.Iterator;

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
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByFirstName(string firstName);

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByLastName(string lastName);

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByDateOfBirth(string dateofbirth);

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Gets last index of records.
        /// </summary>
        /// <returns>Last index of records.</returns>
        public int GetLastIndex();

        /// <summary>
        /// Removes records.
        /// </summary>
        /// <param name="id">Id record to delete.</param>
        /// <returns>Whether the entry has been deleted.</returns>
        public bool Remove(int id);

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge();

        /// <summary>
        /// Gets number of records deleted.
        /// </summary>
        /// <returns>Number of records deleted.</returns>
        public int GetCountDeletedRecords();
    }
}
