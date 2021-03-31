using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

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

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
