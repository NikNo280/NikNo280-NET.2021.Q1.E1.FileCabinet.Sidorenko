using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface that provides valid function.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="record">The record that is checked for correctness.</param>
        public void IsValid(FileCabinetRecord record);

        /// <summary>
        ///  Сhecks a validity of a name.
        /// </summary>
        /// <param name="name">Input name.</param>
        /// <returns>Tuple with result validation and error message.</returns>
        public Tuple<bool, string> NameValidator(string name);

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
