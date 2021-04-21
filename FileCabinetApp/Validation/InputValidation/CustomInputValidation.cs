using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation.InputValidation
{
    /// <summary>
    /// The class provides custom validation rules.
    /// </summary>
    public class CustomInputValidation : IInputValidation
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

            if (firstName.Length < 1 || firstName.Length > 120)
            {
                return new Tuple<bool, string>(false, $"{nameof(firstName.Length)} is less than 1 or bigger than 120");
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

            if (secondName.Length < 1 || secondName.Length > 120)
            {
                return new Tuple<bool, string>(false, $"{nameof(secondName.Length)} is less than 1 or bigger than 120");
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
            if (age <= 18 || age > 110)
            {
                return new Tuple<bool, string>(false, $"{age} is less than 18 or bigger than 110");
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
            if (salary < 1000)
            {
                return new Tuple<bool, string>(false, $"{nameof(salary)} is less than 1000");
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
            if (dateOfBirth >= DateTime.Now || dateOfBirth <= new DateTime(1920, 1, 1))
            {
                return new Tuple<bool, string>(false, $"{nameof(dateOfBirth)} is less than 01-Jan-1920 or greater than current date");
            }

            return new Tuple<bool, string>(true, "ok");
        }
    }
}