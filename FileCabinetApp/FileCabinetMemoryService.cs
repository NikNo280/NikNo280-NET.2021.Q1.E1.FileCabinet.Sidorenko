using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{

    /// <summary>
    /// The class representing functions for interacting with the record model.
    /// </summary>
    public class FileCabinetMemoryService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
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

            this.recordValidator.IsValid(record);

            this.list.Add(record);
            AddToDict(record.FirstName, this.firstNameDictionary, record);
            AddToDict(record.LastName, this.lastNameDictionary, record);
            AddToDict(record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), this.dateOfBirthDictionary, record);

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

            this.recordValidator.IsValid(record);

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

                    UpdateDict(this.firstNameDictionary, item.FirstName, record.FirstName, editRecord);
                    UpdateDict(this.lastNameDictionary, item.LastName, record.LastName, editRecord);
                    UpdateDict(this.dateOfBirthDictionary, item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), record.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), editRecord);
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
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return GetArrayFromDict(firstName, this.firstNameDictionary);
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            return GetArrayFromDict(lastName, this.lastNameDictionary);
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            return GetArrayFromDict(dateofbirth, this.dateOfBirthDictionary);
        }

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.list.ToArray());
        }

        private static ReadOnlyCollection<FileCabinetRecord> GetArrayFromDict(string source, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException($"{nameof(source)} is null or empty");
            }

            if (dictionary.ContainsKey(source.ToUpperInvariant()))
            {
                return new ReadOnlyCollection<FileCabinetRecord>(dictionary[source.ToUpperInvariant()]);
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            }
        }

        private static void AddToDict(string key, Dictionary<string, List<FileCabinetRecord>> dictionary, FileCabinetRecord record)
        {
            if (dictionary.ContainsKey(key.ToUpperInvariant()))
            {
                dictionary[key.ToUpperInvariant()].Add(record);
            }
            else
            {
                dictionary.Add(key.ToUpperInvariant(), new List<FileCabinetRecord>() { record });
            }
        }

        private static void UpdateDict(Dictionary<string, List<FileCabinetRecord>> dictionary, string oldKey, string newKey, FileCabinetRecord record)
        {
            dictionary[oldKey.ToUpperInvariant()].Remove(dictionary[oldKey.ToUpperInvariant()].Where(item => item.Id == record.Id).First());
            AddToDict(newKey, dictionary, record);
        }
    }
}
