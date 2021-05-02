﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Service.Iterator;

namespace FileCabinetApp.Service.Decorator
{
    /// <summary>
    /// Class decorator for IFileCabinetService counting the execution time of functions.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService fileCabinetService;
        private readonly ITicksPrinter printer;
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinetService">FileCabinetService.</param>
        /// <param name="printer">Printer.</param>
        public ServiceMeter(IFileCabinetService fileCabinetService, ITicksPrinter printer)
        {
            this.fileCabinetService = fileCabinetService;
            this.printer = printer;
            this.stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="record">New record.</param>
        /// <returns>Returns new record id.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.stopwatch.Start();
            int value = this.fileCabinetService.CreateRecord(record);
            this.stopwatch.Stop();
            this.printer.Print("Create", this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <summary>
        /// Modify an existing record by id.
        /// </summary>
        /// <param name="record">New record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            this.stopwatch.Start();
            this.fileCabinetService.EditRecord(record);
            this.stopwatch.Stop();
            this.printer.Print("Edit", this.stopwatch.ElapsedTicks);
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByDateOfBirth(string dateofbirth)
        {
            this.stopwatch.Start();
            var value = this.fileCabinetService.FindByDateOfBirth(dateofbirth);
            this.stopwatch.Stop();
            this.printer.Print("Find", this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <summary>
        /// Find records by first name.
        /// </summary>
        /// <param name="firstName">Users first name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            this.stopwatch.Start();
            var value = this.fileCabinetService.FindByFirstName(firstName);
            this.stopwatch.Stop();
            this.printer.Print("Find", this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            this.stopwatch.Start();
            var value = this.fileCabinetService.FindByLastName(lastName);
            this.stopwatch.Stop();
            this.printer.Print("Find", this.stopwatch.ElapsedTicks);
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
            this.stopwatch.Start();
            var value = this.fileCabinetService.GetRecords();
            this.stopwatch.Stop();
            this.printer.Print("List", this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <summary>
        /// Gets count of records.
        /// </summary>
        /// <returns>Return count of records.</returns>
        public int GetStat()
        {
            this.stopwatch.Start();
            var value = this.fileCabinetService.GetStat();
            this.stopwatch.Stop();
            this.printer.Print("Stat", this.stopwatch.ElapsedTicks);
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
            this.stopwatch.Start();
            this.fileCabinetService.Purge();
            this.stopwatch.Stop();
            this.printer.Print("Purge", this.stopwatch.ElapsedTicks);
        }

        /// <summary>
        /// Removes records.
        /// </summary>
        /// <param name="id">Id record to delete.</param>
        /// <returns>Whether the entry has been deleted.</returns>
        public bool Remove(int id)
        {
            this.stopwatch.Start();
            var value = this.fileCabinetService.Remove(id);
            this.stopwatch.Stop();
            this.printer.Print("Remove", this.stopwatch.ElapsedTicks);
            return value;
        }

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            this.stopwatch.Start();
            this.fileCabinetService.Restore(snapshot);
            this.stopwatch.Stop();
            this.printer.Print("Import", this.stopwatch.ElapsedTicks);
        }
    }
}
