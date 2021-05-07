using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
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
        /// Defragments the data file.
        /// </summary>
        public void Purge();

        /// <summary>
        /// Gets number of records deleted.
        /// </summary>
        /// <returns>Number of records deleted.</returns>
        public int GetCountDeletedRecords();

        /// <summary>
        /// Insert record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void InsertRecord(FileCabinetRecord record);

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record.</param>
        /// <returns>Function execution result.</returns>
        public string DeleteRecords(PropertyInfo[] properties, FileCabinetRecord record);

        /// <summary>
        /// Update records.
        /// </summary>
        /// <param name="updateProperties">Properties to update.</param>
        /// <param name="updateRecord">Update record.</param>
        /// <param name="searchProperties">Properties to search.</param>
        /// <param name="searchRecord">Search record.</param>
        public void UpdateRecords(PropertyInfo[] updateProperties, FileCabinetRecord updateRecord, PropertyInfo[] searchProperties, FileCabinetRecord searchRecord);

        /// <summary>
        /// Select records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record to search.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(PropertyInfo[][] properties, FileCabinetRecord[] record);
    }
}
