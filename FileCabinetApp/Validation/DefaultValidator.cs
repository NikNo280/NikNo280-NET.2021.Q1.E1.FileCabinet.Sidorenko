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
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            this.ValidateFirstName(record.FirstName);
            this.ValidateSecondtName(record.FirstName);
            this.ValidateDateOfBirth(record.DateOfBirth);
            this.ValidateAge(record.Age);
            this.ValidateSalary(record.Salary);
            this.ValidateGender(record.Gender);
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        private Tuple<bool, string> ValidateFirstName(string name)
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
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        private Tuple<bool, string> ValidateSecondtName(string name)
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
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        private Tuple<bool, string> ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1950, 1, 1))
            {
                return new Tuple<bool, string>(false, $"{nameof(dateOfBirth)} is less than 01-Jan-1950 or greater than current date");
            }

            return new Tuple<bool, string>(true, "ok");
        }

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="age">Input age.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        private Tuple<bool, string> ValidateAge(short age)
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
        private Tuple<bool, string> ValidateSalary(decimal salary)
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
        private Tuple<bool, string> ValidateGender(char gender)
        {
            if (gender != 'M' && gender != 'W')
            {
                return new Tuple<bool, string>(false, $"{nameof(gender)} gender != 'M' && gender != 'W'");
            }

            return new Tuple<bool, string>(true, "ok");
        }
    }
}
