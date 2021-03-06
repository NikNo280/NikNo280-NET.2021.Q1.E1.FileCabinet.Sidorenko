using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using FileCabinetApp.Service.Iterator;
using NLog;

namespace FileCabinetApp.Service.Decorator
{
    /// <summary>
    /// Class decorator for IFileCabinetService logging function.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        /// <param name="logger">Logger.</param>
        public ServiceLogger(IFileCabinetService fileCabinetService, ILogger logger)
        {
            this.fileCabinetService = fileCabinetService;
            this.logger = logger;
        }

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="record">New record.</param>
        /// <returns>Returns new record id.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now} - Calling Create() with FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth.ToString($"dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Age = '{record.Age}', Salary = '{record.Salary}', Gender = '{record.Gender}'");
            int value = this.fileCabinetService.CreateRecord(record);
            this.logger.Info($"{DateTime.Now} - Create() returned '{value}'");
            return value;
        }

        /// <summary>
        /// Gets number of records deleted.
        /// </summary>
        /// <returns>Number of records deleted.</returns>
        public int GetCountDeletedRecords()
        {
            int value = this.fileCabinetService.GetCountDeletedRecords();
            return value;
        }

        /// <summary>
        /// Gets last index of records.
        /// </summary>
        /// <returns>Last index of records.</returns>
        public int GetLastIndex()
        {
            int value = this.fileCabinetService.GetLastIndex();
            return value;
        }

        /// <summary>
        /// Gets count of records.
        /// </summary>
        /// <returns>Return count of records.</returns>
        public int GetStat()
        {
            this.logger.Info($"{DateTime.Now} - Calling Stat()");
            var value = this.fileCabinetService.GetStat();
            this.logger.Info($"{DateTime.Now} - Stat() ended");
            return value;
        }

        /// <summary>
        /// Insert record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void InsertRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now} - Calling Insert() with FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth.ToString($"dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Age = '{record.Age}', Salary = '{record.Salary}', Gender = '{record.Gender}'");
            this.fileCabinetService.InsertRecord(record);
            this.logger.Info($"{DateTime.Now} - Insert() ended");
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record.</param>
        /// <returns>Function execution result.</returns>
        public string DeleteRecords(PropertyInfo[] properties, FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now} - Calling Delete() with FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth.ToString($"dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Age = '{record.Age}', Salary = '{record.Salary}', Gender = '{record.Gender}'");
            string result = this.fileCabinetService.DeleteRecords(properties, record);
            this.logger.Info($"{DateTime.Now} - Delete() ended");
            return result;
        }

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var value = this.fileCabinetService.MakeSnapshot();
            return value;
        }

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge()
        {
            this.logger.Info($"{DateTime.Now} - Calling Purge()");
            this.fileCabinetService.Purge();
            this.logger.Info($"{DateTime.Now} - Purge() ended");
        }

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.logger.Info($"{DateTime.Now} - Calling Import() with snapshot");
            this.fileCabinetService.Restore(snapshot);
            this.logger.Info($"{DateTime.Now} - Import() ended");
        }

        /// <summary>
        /// Update records.
        /// </summary>
        /// <param name="updateProperties">Properties to update.</param>
        /// <param name="updateRecord">Update record.</param>
        /// <param name="searchProperties">Properties to search.</param>
        /// <param name="searchRecord">Search record.</param>
        public void UpdateRecords(PropertyInfo[] updateProperties, FileCabinetRecord updateRecord, PropertyInfo[] searchProperties, FileCabinetRecord searchRecord)
        {
            this.logger.Info($"{DateTime.Now} - Calling Update() with snapshot");
            this.fileCabinetService.UpdateRecords(updateProperties, updateRecord, searchProperties, searchRecord);
            this.logger.Info($"{DateTime.Now} - Update() ended");
        }

        /// <summary>
        /// Select records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record to search.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(PropertyInfo[][] properties, FileCabinetRecord[] record)
        {
            this.logger.Info($"{DateTime.Now} - Calling Select() with snapshot");
            var iterator = this.fileCabinetService.SelectRecords(properties, record);
            this.logger.Info($"{DateTime.Now} - Select() ended");
            return iterator;
        }
    }
}
