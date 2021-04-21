using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation.InputValidation
{
    /// <summary>
    /// The class provides default validation rules.
    /// </summary>
    public class DefaultInputValidation : IInputValidation
    {
        /// <summary>
        ///  Сhecks a validity of a first name.
        /// </summary>
        /// <param name="firstName">Input first name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> FirstNameValidator(string firstName)
        {
            if (firstName is null)
            {
                return new Tuple<bool, string>(false, $"{nameof(firstName)} is null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                return new Tuple<bool, string>(false, $"{nameof(firstName.Length)} is less than 2 or bigger than 60");
            }

            return new Tuple<bool, string>(true, firstName);
        }

        /// <summary>
        ///  Сhecks a validity of a second name.
        /// </summary>
        /// <param name="secondName">Input second name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> SecondNameValidator(string secondName)
        {
            if (secondName is null)
            {
                return new Tuple<bool, string>(false, $"{nameof(secondName)} is null");
            }

            if (secondName.Length < 2 || secondName.Length > 60)
            {
                return new Tuple<bool, string>(false, $"{nameof(secondName.Length)} is less than 2 or bigger than 60");
            }

            return new Tuple<bool, string>(true, secondName);
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