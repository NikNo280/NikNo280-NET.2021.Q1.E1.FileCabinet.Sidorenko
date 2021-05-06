using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.Service.Iterator;
using FileCabinetApp.Service.Iterator.Enumerable;

namespace FileCabinetApp
{
    /// <summary>
    /// The class representing functions for interacting with the record model.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.recordValidator = recordValidator;
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
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Incorrect prarameters");
            }

            this.list.Add(record);
            AddToDict(record.FirstName.ToUpperInvariant(), this.firstNameDictionary, record);
            AddToDict(record.LastName.ToUpperInvariant(), this.lastNameDictionary, record);
            AddToDict(record.DateOfBirth, this.dateOfBirthDictionary, record);

            return record.Id;
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Gets count of records.
        /// </summary>
        /// <returns>Return count of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Modify an existing record by id.
        /// </summary>
        /// <param name="record">New record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Incorrect prarameters");
            }

            foreach (var item in this.list)
            {
                if (item.Id == record.Id)
                {
                    var editRecord = new FileCabinetRecord
                    {
                        Id = record.Id,
                        FirstName = record.FirstName,
                        LastName = record.LastName,
                        DateOfBirth = record.DateOfBirth,
                        Age = record.Age,
                        Salary = record.Salary,
                        Gender = record.Gender,
                    };

                    UpdateDict(this.firstNameDictionary, item.FirstName.ToUpperInvariant(), record.FirstName.ToUpperInvariant(), editRecord);
                    UpdateDict(this.lastNameDictionary, item.LastName.ToUpperInvariant(), record.LastName.ToUpperInvariant(), editRecord);
                    UpdateDict(this.dateOfBirthDictionary, item.DateOfBirth, record.DateOfBirth, editRecord);
                    this.list.Remove(item);
                    this.list.Add(editRecord);
                    return;
                }
            }

            throw new ArgumentException($"#{record.Id} record is not found.");
        }

        /// <summary>
        /// Find records by first name.
        /// </summary>
        /// <param name="firstName">Users first name.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException($"{nameof(firstName)} is null or empty");
            }

            return GetIteratorFromDict(firstName.ToUpperInvariant(), this.firstNameDictionary);
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException($"{nameof(lastName)} is null or empty");
            }

            return GetIteratorFromDict(lastName.ToUpperInvariant(), this.lastNameDictionary);
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            if (string.IsNullOrWhiteSpace(dateofbirth))
            {
                throw new ArgumentException($"{nameof(dateofbirth)} is null or empty");
            }

            DateTime dateTime;
            bool result = DateTime.TryParseExact(dateofbirth, "yyyy-MMM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            if (result)
            {
                return GetIteratorFromDict(dateTime, this.dateOfBirthDictionary);
            }
            else
            {
                return new MemoryEnumerable(Array.Empty<FileCabinetRecord>());
            }
        }

        /// <summary>
        /// Insert record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void InsertRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (this.IsRecordById(record.Id))
            {
                this.EditRecord(record);
            }
            else
            {
                this.CreateRecord(record);
            }
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record.</param>
        /// <returns>Function execution result.</returns>
        public string DeleteRecords(PropertyInfo[] properties, FileCabinetRecord record)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            bool result = true;
            List<int> removeRecordId = new List<int>();
            for (int i = 0; i < this.list.Count; i++)
            {
                result = true;
                var deleteRecord = this.list[i];
                foreach (var property in properties)
                {
                    if (!property.GetValue(record).Equals(property.GetValue(deleteRecord)))
                    {
                        result = false;
                    }
                }

                if (result)
                {
                    removeRecordId.Add(deleteRecord.Id);
                }
            }

            var stringBuilder = new StringBuilder();
            if (removeRecordId.Count == 0)
            {
                return "No record has been deleted";
            }
            else if (removeRecordId.Count == 1)
            {
                stringBuilder.Append("Record ");
            }
            else
            {
                stringBuilder.Append("Records ");
            }

            foreach (var id in removeRecordId)
            {
                stringBuilder.Append($"#{id}, ");
                this.Remove(id);
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            if (removeRecordId.Count == 1)
            {
                stringBuilder.Append("is deleted.");
            }
            else
            {
                stringBuilder.Append("are deleted.");
            }

            return stringBuilder.ToString();
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
            if (updateProperties is null)
            {
                throw new ArgumentNullException(nameof(updateProperties));
            }

            if (updateRecord is null)
            {
                throw new ArgumentNullException(nameof(updateRecord));
            }

            if (searchProperties is null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            if (searchRecord is null)
            {
                throw new ArgumentNullException(nameof(searchRecord));
            }

            bool result;
            for (int i = 0; i < this.list.Count; i++)
            {
                result = true;
                var record = this.list[i];
                foreach (var searchProperty in searchProperties)
                {
                    if (!searchProperty.GetValue(searchRecord).Equals(searchProperty.GetValue(record)))
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    var createNewRecord = new FileCabinetRecord();
                    foreach (var property in typeof(FileCabinetRecord).GetProperties())
                    {
                        property.SetValue(createNewRecord, property.GetValue(record));
                    }

                    foreach (var updateProperty in updateProperties)
                    {
                        updateProperty.SetValue(createNewRecord, updateProperty.GetValue(updateRecord));
                    }

                    this.EditRecord(createNewRecord);
                }
            }
        }

        /// <summary>
        /// looking for an record with this ID exists.
        /// </summary>
        /// <param name="id">Records id.</param>
        /// <returns>Is there a record with such an identifier.</returns>
        public bool IsRecordById(int id)
        {
            return this.list.Any(recod => recod.Id == id);
        }

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
        }

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} is null");
            }

            var id_dictionary = new Dictionary<int, FileCabinetRecord>();

            foreach (var record in this.list)
            {
                id_dictionary.Add(record.Id, record);
            }

            foreach (var record in snapshot.Records)
            {
                if (id_dictionary.ContainsKey(record.Id))
                {
                    this.EditRecord(record);
                }
                else
                {
                    this.CreateRecord(record);
                }
            }
        }

        /// <summary>
        /// Gets last index of records.
        /// </summary>
        /// <returns>Last index of records.</returns>
        public int GetLastIndex()
        {
            return this.list.Count == 0 ? 0 : this.list.Max(record => record.Id);
        }

        /// <summary>
        /// Removes records.
        /// </summary>
        /// <param name="id">Id record to delete.</param>
        /// <returns>Whether the entry has been deleted.</returns>
        public bool Remove(int id)
        {
            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    this.list.Remove(record);
                    RemoveRecordInDict(this.firstNameDictionary, record, record.FirstName);
                    RemoveRecordInDict(this.lastNameDictionary, record, record.LastName);
                    RemoveRecordInDict(this.dateOfBirthDictionary, record, record.DateOfBirth);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge()
        {
        }

        /// <summary>
        /// Gets number of records deleted.
        /// </summary>
        /// <returns>Number of records deleted.</returns>
        public int GetCountDeletedRecords()
        {
            return 0;
        }

        private static void RemoveRecordInDict<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record, T key)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (dictionary is null)
            {
                throw new ArgumentNullException($"{nameof(dictionary)} is null");
            }

            if (dictionary.ContainsKey(key))
            {
                var fileCabinetRecords = dictionary[key];
                if (fileCabinetRecords.Contains(record))
                {
                    if (fileCabinetRecords.Count == 1)
                    {
                        dictionary.Remove(key);
                    }
                    else
                    {
                        fileCabinetRecords.Remove(record);
                    }
                }
            }
        }

        private static IEnumerable<FileCabinetRecord> GetIteratorFromDict<T>(T source, Dictionary<T, List<FileCabinetRecord>> dictionary)
        {
            if (dictionary.ContainsKey(source))
            {
                return new MemoryEnumerable(dictionary[source].ToArray());
            }
            else
            {
                return new MemoryEnumerable(Array.Empty<FileCabinetRecord>());
            }
        }

        private static void AddToDict<T>(T key, Dictionary<T, List<FileCabinetRecord>> dictionary, FileCabinetRecord record)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(record);
            }
            else
            {
                dictionary.Add(key, new List<FileCabinetRecord>() { record });
            }
        }

        private static void UpdateDict<T>(Dictionary<T, List<FileCabinetRecord>> dictionary, T oldKey, T newKey, FileCabinetRecord record)
        {
            dictionary[oldKey].Remove(dictionary[oldKey].Where(item => item.Id == record.Id).First());
            AddToDict(newKey, dictionary, record);
        }
    }
}
