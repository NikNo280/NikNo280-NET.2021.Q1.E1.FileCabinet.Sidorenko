using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation.InputValidation
{
    /// <summary>
    /// Interface that provides valid function.
    /// </summary>
    public interface IInputValidation
    {
        /// <summary>
        ///  Сhecks a validity of a first name.
        /// </summary>
        /// <param name="firstName">Input first name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> FirstNameValidator(string firstName);

        /// <summary>
        ///  Сhecks a validity of a second name.
        /// </summary>
        /// <param name="secondName">Input second name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> SecondNameValidator(string secondName);

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="age">Input age.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> AgeValidator(short age);

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="salary">Input salary.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> SalaryValidator(decimal salary);

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="gender">Input gender.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> GenderValidator(char gender);

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="dateOfBirth">Input dateOfBirth.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> DateOfBirthValidator(DateTime dateOfBirth);
    }
}
