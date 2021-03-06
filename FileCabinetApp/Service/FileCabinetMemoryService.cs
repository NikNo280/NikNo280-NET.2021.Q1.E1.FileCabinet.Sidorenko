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
        private readonly List<FileCabinetRecord[]> selectParams = new List<FileCabinetRecord[]>();
        private readonly List<IEnumerable<FileCabinetRecord>> selectResult = new List<IEnumerable<FileCabinetRecord>>();
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

            /*
            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Incorrect prarameters");
            }
            */
            this.list.Add(record);
            AddToDict(record.FirstName.ToUpperInvariant(), this.firstNameDictionary, record);
            AddToDict(record.LastName.ToUpperInvariant(), this.lastNameDictionary, record);
            AddToDict(record.DateOfBirth, this.dateOfBirthDictionary, record);
            this.selectResult.Clear();
            this.selectParams.Clear();
            return record.Id;
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
                    UpdateDict(this.firstNameDictionary, item.FirstName.ToUpperInvariant(), record.FirstName.ToUpperInvariant(), record);
                    UpdateDict(this.lastNameDictionary, item.LastName.ToUpperInvariant(), record.LastName.ToUpperInvariant(), record);
                    UpdateDict(this.dateOfBirthDictionary, item.DateOfBirth, record.DateOfBirth, record);

                    item.Id = record.Id;
                    item.FirstName = record.FirstName;
                    item.LastName = record.LastName;
                    item.DateOfBirth = record.DateOfBirth;
                    item.Age = record.Age;
                    item.Salary = record.Salary;

                    this.selectResult.Clear();
                    this.selectParams.Clear();
                    return;
                }
            }

            throw new ArgumentException($"#{record.Id} record is not found.");
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

            const string ZeroRecordsToDelete = "No record has been deleted";
            var firstNameProperty = typeof(FileCabinetRecord).GetProperty("FirstName");
            var lastNameProperty = typeof(FileCabinetRecord).GetProperty("LastName");
            var dateOfBirthProperty = typeof(FileCabinetRecord).GetProperty("DateOfBirth");
            var removeRecordId = new List<int>();
            if (properties.Contains(firstNameProperty))
            {
                string recordFirstNameValue = (string)firstNameProperty.GetValue(record);
                if (this.firstNameDictionary.ContainsKey(recordFirstNameValue.ToUpperInvariant()))
                {
                    foreach (var item in this.firstNameDictionary[recordFirstNameValue.ToUpperInvariant()])
                    {
                        if (RecordEqualsByProperties(record, item, properties))
                        {
                            removeRecordId.Add(item.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else if (properties.Contains(lastNameProperty))
            {
                string recordLastNameValue = (string)lastNameProperty.GetValue(record);
                if (this.lastNameDictionary.ContainsKey(recordLastNameValue.ToUpperInvariant()))
                {
                    foreach (var item in this.lastNameDictionary[recordLastNameValue.ToUpperInvariant()])
                    {
                        if (RecordEqualsByProperties(record, item, properties))
                        {
                            removeRecordId.Add(item.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else if (properties.Contains(dateOfBirthProperty))
            {
                DateTime recordDateOfBirthValue = (DateTime)dateOfBirthProperty.GetValue(record);
                if (this.dateOfBirthDictionary.ContainsKey(recordDateOfBirthValue))
                {
                    foreach (var item in this.dateOfBirthDictionary[recordDateOfBirthValue])
                    {
                        if (RecordEqualsByProperties(record, item, properties))
                        {
                            removeRecordId.Add(item.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    var deleteRecord = this.list[i];
                    if (RecordEqualsByProperties(record, deleteRecord, properties))
                    {
                        removeRecordId.Add(deleteRecord.Id);
                    }
                }
            }

            var stringBuilder = new StringBuilder();
            if (removeRecordId.Count == 0)
            {
                return ZeroRecordsToDelete;
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

            var firstNameProperty = typeof(FileCabinetRecord).GetProperty("FirstName");
            var lastNameProperty = typeof(FileCabinetRecord).GetProperty("LastName");
            var dateOfBirthProperty = typeof(FileCabinetRecord).GetProperty("DateOfBirth");
            if (searchProperties.Contains(firstNameProperty))
            {
                string recordFirstNameValue = (string)firstNameProperty.GetValue(searchRecord);
                if (this.firstNameDictionary.ContainsKey(recordFirstNameValue.ToUpperInvariant()))
                {
                    var collection = this.firstNameDictionary[recordFirstNameValue.ToUpperInvariant()].ToArray();
                    foreach (var record in collection)
                    {
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
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
            }
            else if (searchProperties.Contains(lastNameProperty))
            {
                string recordLastNameValue = (string)lastNameProperty.GetValue(searchRecord);
                if (this.lastNameDictionary.ContainsKey(recordLastNameValue.ToUpperInvariant()))
                {
                    var collection = this.lastNameDictionary[recordLastNameValue.ToUpperInvariant()].ToArray();
                    foreach (var record in collection)
                    {
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
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
            }
            else if (searchProperties.Contains(dateOfBirthProperty))
            {
                DateTime recordDateOfBirthValue = (DateTime)dateOfBirthProperty.GetValue(searchRecord);
                if (this.dateOfBirthDictionary.ContainsKey(recordDateOfBirthValue))
                {
                    var collection = this.dateOfBirthDictionary[recordDateOfBirthValue].ToArray();
                    foreach (var record in collection)
                    {
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
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
            }
            else
            {
                for (int i = 0; i < this.list.Count; i++)
                {
                    if (RecordEqualsByProperties(this.list[i], searchRecord, searchProperties))
                    {
                        var createNewRecord = new FileCabinetRecord();
                        foreach (var property in typeof(FileCabinetRecord).GetProperties())
                        {
                            property.SetValue(createNewRecord, property.GetValue(this.list[i]));
                        }

                        foreach (var updateProperty in updateProperties)
                        {
                            updateProperty.SetValue(createNewRecord, updateProperty.GetValue(updateRecord));
                        }

                        this.EditRecord(createNewRecord);
                    }
                }
            }
        }

        /// <summary>
        /// Select records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record to search.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(PropertyInfo[][] properties, FileCabinetRecord[] record)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            if (properties.Length == 0)
            {
                return new MemoryEnumerable(this.list.ToArray());
            }

            var records = new List<FileCabinetRecord>();
            var memoisation = this.GetMemoization(properties, record);

            if (memoisation != -1)
            {
                return this.selectResult[memoisation];
            }

            for (int i = 0; i < this.list.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    if (RecordEqualsByProperties(this.list[i], record[j], properties[j]))
                    {
                        records.Add(this.list[i]);
                        break;
                    }
                }
            }

            var memoryEnumerable = new MemoryEnumerable(records.ToArray());
            this.selectParams.Add(record);
            this.selectResult.Add(memoryEnumerable);
            return memoryEnumerable;
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
                    this.selectResult.Clear();
                    this.selectParams.Clear();
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
            if (dictionary[oldKey].Count == 0)
            {
                dictionary.Remove(oldKey);
            }

            AddToDict(newKey, dictionary, record);
        }

        private static bool RecordEqualsByProperties(FileCabinetRecord record1, FileCabinetRecord record2, PropertyInfo[] properties)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record1 is null || record2 is null)
            {
                return false;
            }

            bool result = true;
            foreach (var property in properties)
            {
                var value1 = property.GetValue(record1);
                var value2 = property.GetValue(record2);
                if (value1 is null && value2 is null)
                {
                    continue;
                }
                else if (value1 is null && !(value2 is null))
                {
                    result = false;
                    break;
                }
                else if (!(value1 is null) && value2 is null)
                {
                    result = false;
                    break;
                }
                else if (!property.GetValue(record1).Equals(property.GetValue(record2)))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private int GetMemoization(PropertyInfo[][] properties, FileCabinetRecord[] records)
        {
            bool finalResult = false;
            for (int k = 0; k < this.selectParams.Count; k++)
            {
                finalResult = false;
                if (this.selectParams[k].Length == records.Length)
                {
                    for (int i = 0; i < this.selectParams[k].Length; i++)
                    {
                        bool result = false;
                        for (int j = 0; j < records.Length; j++)
                        {
                            if (RecordEqualsByProperties(this.selectParams[k][i], records[j], properties[j]))
                            {
                                result = true;
                            }
                        }

                        if (!result)
                        {
                            finalResult = false;
                            break;
                        }

                        finalResult = true;
                    }
                }

                if (finalResult)
                {
                    return k;
                }
            }

            return -1;
        }
    }
}
