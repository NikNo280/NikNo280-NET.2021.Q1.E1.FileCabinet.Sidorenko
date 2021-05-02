using System;
using System.Collections.ObjectModel;
using System.Globalization;
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
        /// Modify an existing record by id.
        /// </summary>
        /// <param name="record">New record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.logger.Info($"{DateTime.Now} - Calling Edit() with FirstName = '{record.FirstName}', LastName = '{record.LastName}', " +
                $"DateOfBirth = '{record.DateOfBirth.ToString($"dd/MM/yyyy", CultureInfo.InvariantCulture)}', " +
                $"Age = '{record.Age}', Salary = '{record.Salary}', Gender = '{record.Gender}'");
            this.fileCabinetService.EditRecord(record);
            this.logger.Info($"{DateTime.Now} - Edit() ended");
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByDateOfBirth(string dateofbirth)
        {
            this.logger.Info($"{DateTime.Now} - Calling Find() with dateofbirth = '{dateofbirth}'");
            var value = this.fileCabinetService.FindByDateOfBirth(dateofbirth);
            this.logger.Info($"{DateTime.Now} - Find() ended");
            return value;
        }

        /// <summary>
        /// Find records by first name.
        /// </summary>
        /// <param name="firstName">Users first name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            this.logger.Info($"{DateTime.Now} - Calling Find() with firstName = '{firstName}'");
            var value = this.fileCabinetService.FindByFirstName(firstName);
            this.logger.Info($"{DateTime.Now} - Find() ended");
            return value;
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            this.logger.Info($"{DateTime.Now} - Calling Find() with lastName = '{lastName}'");
            var value = this.fileCabinetService.FindByLastName(lastName);
            this.logger.Info($"{DateTime.Now} - Find() ended");
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
        /// Gets all records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.logger.Info($"{DateTime.Now} - Calling List()");
            var value = this.fileCabinetService.GetRecords();
            this.logger.Info($"{DateTime.Now} - List() ended");
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
        /// Removes records.
        /// </summary>
        /// <param name="id">Id record to delete.</param>
        /// <returns>Whether the entry has been deleted.</returns>
        public bool Remove(int id)
        {
            this.logger.Info($"{DateTime.Now} - Calling Find() with id = '{id}'");
            var value = this.fileCabinetService.Remove(id);
            this.logger.Info($"{DateTime.Now} - Remove() ended");
            return value;
        }

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.logger.Info($"{DateTime.Now} - Calling Find() with snapshot");
            this.fileCabinetService.Restore(snapshot);
            this.logger.Info($"{DateTime.Now} - Restore() ended");
        }
    }
}
