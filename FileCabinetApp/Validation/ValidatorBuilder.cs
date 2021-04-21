using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Validator builder class.
    /// </summary>
    public class ValidatorBuilder
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBuilder"/> class.
        /// </summary>
        public ValidatorBuilder()
        {
            this.validators = new List<IRecordValidator>();
        }

        /// <summary>
        /// Create CompositeValidator class with validators.
        /// </summary>
        /// <returns>CompositeValidator class.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Add first name validator.
        /// </summary>
        /// <param name="min">Min length.</param>
        /// <param name="max">Max length.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add second name validator.
        /// </summary>
        /// <param name="min">Min length.</param>
        /// <param name="max">Max length.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add date of birth validator.
        /// </summary>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Add age validator.
        /// </summary>
        /// <param name="min">Min age.</param>
        /// <param name="max">Max age.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateAge(short min, short max)
        {
            this.validators.Add(new AgeValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add salary validator.
        /// </summary>
        /// <param name="min">Min salary.</param>
        /// <param name="max">Max salary.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateSalary(decimal min, decimal max)
        {
            this.validators.Add(new SalaryValidator(min, max));
            return this;
        }

        /// <summary>
        /// Add gender validator.
        /// </summary>
        /// <param name="genders">Array of genders.</param>
        /// <returns>This ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateGender(char[] genders)
        {
            this.validators.Add(new GenderValidator(genders));
            return this;
        }
    }
}
