using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Date of birth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Date from.</param>
        /// <param name="to">Date to.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Сhecks the validity of the data.
        /// </summary>
        /// <param name="parameters">Parameters that are checked for correctness.</param>
        /// <returns>Whether the parameter meets the specified criteria.</returns>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            return parameters.DateOfBirth >= this.from && parameters.DateOfBirth <= this.to;
        }
    }
}
