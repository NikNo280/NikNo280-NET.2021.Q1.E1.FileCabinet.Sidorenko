using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Validation.Extension
{
    /// <summary>
    /// Provides extension methods for ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtension
    {
        /// <summary>
        /// Config.
        /// </summary>
        private static readonly IConfiguration Config = new ConfigurationBuilder()
            .AddJsonFile("validation-rules.json", true, true)
            .Build();

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
                .ValidateFirstName(int.Parse(Config["default:firstName:min"], CultureInfo.InvariantCulture), int.Parse(Config["default:firstName:max"], CultureInfo.InvariantCulture))
                .ValidateLastName(int.Parse(Config["default:lastName:min"], CultureInfo.InvariantCulture), int.Parse(Config["default:lastName:max"], CultureInfo.InvariantCulture))
                .ValidateDateOfBirth(
                    DateTime.ParseExact(Config["default:dateOfBirth:from"], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DateTime.ParseExact(Config["default:dateOfBirth:to"], "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .ValidateAge(short.Parse(Config["default:age:min"], CultureInfo.InvariantCulture), short.Parse(Config["default:age:max"], CultureInfo.InvariantCulture))
                .ValidateSalary(decimal.Parse(Config["default:salary:min"], CultureInfo.InvariantCulture), decimal.Parse(Config["default:salary:max"], CultureInfo.InvariantCulture))
                .ValidateGender(Config["default:genders"].ToCharArray())
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
                .ValidateFirstName(int.Parse(Config["custom:firstName:min"], CultureInfo.InvariantCulture), int.Parse(Config["custom:firstName:max"], CultureInfo.InvariantCulture))
                .ValidateLastName(int.Parse(Config["custom:lastName:min"], CultureInfo.InvariantCulture), int.Parse(Config["custom:lastName:max"], CultureInfo.InvariantCulture))
                .ValidateDateOfBirth(
                    DateTime.ParseExact(Config["custom:dateOfBirth:from"], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DateTime.ParseExact(Config["custom:dateOfBirth:to"], "dd/MM/yyyy", CultureInfo.InvariantCulture))
                .ValidateAge(short.Parse(Config["custom:age:min"], CultureInfo.InvariantCulture), short.Parse(Config["custom:age:max"], CultureInfo.InvariantCulture))
                .ValidateSalary(decimal.Parse(Config["custom:salary:min"], CultureInfo.InvariantCulture), decimal.Parse(Config["custom:salary:max"], CultureInfo.InvariantCulture))
                .ValidateGender(Config["custom:genders"].ToCharArray())
                .Create();
        }
    }
}
