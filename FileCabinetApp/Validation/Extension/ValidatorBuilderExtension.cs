using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation.Extension
{
    /// <summary>
    /// Provides extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>
        /// Create default validator.
        /// </summary>
        /// <param name="validatorBuilder">ValidatorBuilder.</param>
        /// <returns>Record validator.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }

            return validatorBuilder
                .ValidateFirstName(2, 60)
                .ValidateLastName(2, 60)
                .ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now)
                .ValidateAge(1, 72)
                .ValidateSalary(0, decimal.MaxValue)
                .ValidateGender("MW".ToCharArray())
                .Create();
        }

        /// <summary>
        /// Create custom validator.
        /// </summary>
        /// <param name="validatorBuilder">ValidatorBuilder.</param>
        /// <returns>Record validator.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validatorBuilder)
        {
            if (validatorBuilder is null)
            {
                throw new ArgumentNullException(nameof(validatorBuilder));
            }

            return validatorBuilder
                .ValidateFirstName(2, 100)
                .ValidateLastName(2, 100)
                .ValidateDateOfBirth(new DateTime(1900, 1, 1), DateTime.Now)
                .ValidateAge(1, 120)
                .ValidateSalary(0, 999999)
                .ValidateGender("MWO".ToCharArray())
                .Create();
        }
    }
}
