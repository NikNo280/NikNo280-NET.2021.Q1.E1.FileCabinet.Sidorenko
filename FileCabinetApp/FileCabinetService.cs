using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class representing functions for interacting with the record model.
    /// </summary>
    public class FileCabinetService : IRecordValidator
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

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

            this.IsValid(record);

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
        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
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

            this.IsValid(record);

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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            return GetArrayFromDict(firstName, this.firstNameDictionary);
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            return GetArrayFromDict(lastName, this.lastNameDictionary);
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Array of records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateofbirth)
        {
            return GetArrayFromDict(dateofbirth, this.dateOfBirthDictionary);
        }

        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="record">The record that is checked for correctness.</param>
        protected virtual void IsValid(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (record.FirstName is null)
            {
                throw new ArgumentNullException($"{nameof(record.FirstName)} is null");
            }

            if (record.LastName is null)
            {
                throw new ArgumentNullException($"{nameof(record.LastName)} is null");
            }

            if (record.FirstName.Length <= 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(record.FirstName.Length)} is less than 2 or bigger than 60");
            }

            if (record.LastName.Length <= 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(record.LastName.Length)} is less than 2 or bigger than 60");
            }

            if (record.FirstName.Equals(new string(' ', record.FirstName.Length)))
            {
                throw new ArgumentException($"{nameof(record.FirstName)} consists only of spaces");
            }

            if (record.LastName.Equals(new string(' ', record.LastName.Length)))
            {
                throw new ArgumentException($"{nameof(record.LastName)} consists only of spaces");
            }

            if (record.Age < 0 || record.Age > 110)
            {
                throw new ArgumentException($"{nameof(record.Age)} is less than zero or bigger than 110");
            }

            if (record.Salary < 0)
            {
                throw new ArgumentException($"{nameof(record.Salary)} is less than zero");
            }

            if (record.DateOfBirth >= DateTime.Now || record.DateOfBirth <= new DateTime(1950, 1, 1))
            {
                throw new ArgumentException($"{nameof(record.DateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            if (record.Gender != 'M' && record.Gender != 'W')
            {
                throw new ArgumentException($"{nameof(record.Gender)} gender != 'M' && gender != 'W'");
            }
        }

        private static FileCabinetRecord[] GetArrayFromDict(string source, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException($"{nameof(source)} is null or empty");
            }

            if (dictionary.ContainsKey(source.ToUpperInvariant()))
            {
                return dictionary[source.ToUpperInvariant()].ToArray();
            }
            else
            {
                return Array.Empty<FileCabinetRecord>();
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

        void IRecordValidator.IsValid(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }
    }
}
