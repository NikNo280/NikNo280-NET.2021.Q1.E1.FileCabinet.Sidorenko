using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class provides default validation rules.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="record">The record that is checked for correctness.</param>
        public void IsValid(FileCabinetRecord record)
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

            if (record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException($"{nameof(record.FirstName.Length)} is less than 2 or bigger than 60");
            }

            if (record.LastName.Length < 2 || record.LastName.Length > 60)
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

            if (record.Age <= 0 || record.Age > 110)
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

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> NameValidator(string name)
        {
            if (name is null)
            {
                return new Tuple<bool, string>(false, $"{nameof(name)} is null");
            }

            if (name.Length < 2 || name.Length > 60)
            {
                return new Tuple<bool, string>(false, $"{nameof(name.Length)} is less than 2 or bigger than 60");
            }

            return new Tuple<bool, string>(true, name);
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="age">Input age.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> AgeValidator(short age)
        {
            if (age < 0 || age > 110)
            {
                return new Tuple<bool, string>(false, $"{age} is less than zero or bigger than 110");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="salary">Input salary.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> SalaryValidator(decimal salary)
        {
            if (salary < 0)
            {
                return new Tuple<bool, string>(false, $"{nameof(salary)} is less than zero");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="gender">Input gender.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> GenderValidator(char gender)
        {
            if (gender != 'M' && gender != 'W')
            {
                return new Tuple<bool, string>(false, $"{nameof(gender)} gender != 'M' && gender != 'W'");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth)
        {
            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1950, 1, 1))
            {
                return new Tuple<bool, string>(false, $"{nameof(dateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            return new Tuple<bool, string>(true, "ok");
        }
    }
}
