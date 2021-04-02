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
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="firstName">User first name.</param>
        /// <param name="lastName">User last name.</param>
        /// <param name="dateOfBirth">User date of birth.</param>
        /// <param name="age">User age.</param>
        /// <param name="salary">User salary.</param>
        /// <param name="gender">User gender.</param>
        /// <returns>Returns new record id.</returns>
        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null");
            }

            if (lastName is null)
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null");
            }

            if (firstName.Length <= 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)} is less than 2 or bigger than 60");
            }

            if (lastName.Length <= 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)} is less than 2 or bigger than 60");
            }

            if (firstName.Equals(new string(' ', firstName.Length)))
            {
                throw new ArgumentException($"{nameof(lastName)} consists only of spaces");
            }

            if (lastName.Equals(new string(' ', lastName.Length)))
            {
                throw new ArgumentException($"{nameof(lastName)} consists only of spaces");
            }

            if (age < 0 || age > 110)
            {
                throw new ArgumentException($"{nameof(age)} is less than zero or bigger than 110");
            }

            if (salary < 0)
            {
                throw new ArgumentException($"{nameof(salary)} is less than zero");
            }

            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1950, 1, 1))
            {
                throw new ArgumentException($"{nameof(dateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            if (gender != 'M' && gender != 'W')
            {
                throw new ArgumentException($"{nameof(gender)} gender != 'M' && gender != 'W'");
            }

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Age = age,
                Salary = salary,
                Gender = gender,
            };

            this.list.Add(record);
            AddToDict(firstName, this.firstNameDictionary, record);
            AddToDict(lastName, this.lastNameDictionary, record);
            AddToDict(dateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), this.dateOfBirthDictionary, record);

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
        /// <param name="id">Record id.</param>
        /// <param name="firstName">User first name.</param>
        /// <param name="lastName">User last name.</param>
        /// <param name="dateOfBirth">User date of birth.</param>
        /// <param name="age">User age.</param>
        /// <param name="salary">User salary.</param>
        /// <param name="gender">User gender.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short age, decimal salary, char gender)
        {
            if (firstName is null)
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null");
            }

            if (lastName is null)
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null");
            }

            if (firstName.Length <= 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(firstName)} is less than 2 or bigger than 60");
            }

            if (lastName.Length <= 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"{nameof(lastName)} is less than 2 or bigger than 60");
            }

            if (firstName.Equals(new string(' ', firstName.Length)))
            {
                throw new ArgumentException($"{nameof(lastName)} consists only of spaces");
            }

            if (lastName.Equals(new string(' ', lastName.Length)))
            {
                throw new ArgumentException($"{nameof(lastName)} consists only of spaces");
            }

            if (age < 0 || age > 110)
            {
                throw new ArgumentException($"{nameof(age)} is less than zero or bigger than 110");
            }

            if (salary < 0)
            {
                throw new ArgumentException($"{nameof(salary)} is less than zero");
            }

            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1950, 1, 1))
            {
                throw new ArgumentException($"{nameof(dateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            if (gender != 'M' && gender != 'W')
            {
                throw new ArgumentException($"{nameof(gender)} gender != 'M' && gender != 'W'");
            }

            foreach (var item in this.list)
            {
                if (item.Id == id)
                {
                    var editRecord = new FileCabinetRecord
                    {
                        Id = id,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Age = age,
                        Salary = salary,
                        Gender = gender,
                    };

                    UpdateDict(this.firstNameDictionary, item.FirstName, firstName, editRecord);
                    UpdateDict(this.lastNameDictionary, item.LastName, lastName, editRecord);
                    UpdateDict(this.dateOfBirthDictionary, item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture), editRecord);
                    this.list.Remove(item);
                    this.list.Add(editRecord);
                    return;
                }
            }

            throw new ArgumentException($"#{id} record is not found.");
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
    }
}
