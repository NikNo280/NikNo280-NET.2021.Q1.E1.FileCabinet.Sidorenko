using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Salary validator.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal max;
        private readonly decimal min;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="min">Min salary.</param>
        /// <param name="max">Max salary.</param>
        public SalaryValidator(decimal min, decimal max)
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

            return parameters.Salary >= this.min && parameters.Salary <= this.max;
        }
    }
}
