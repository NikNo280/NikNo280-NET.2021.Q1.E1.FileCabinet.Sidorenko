using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Age validator.
    /// </summary>
    public class AgeValidator : IRecordValidator
    {
        private readonly short max;
        private readonly short min;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgeValidator"/> class.
        /// </summary>
        /// <param name="min">Min age.</param>
        /// <param name="max">Max age.</param>
        public AgeValidator(short min, short max)
        {
            this.min = min;
            this.max = max;
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

            return parameters.Age >= this.min && parameters.Age <= this.max;
        }
    }
}
