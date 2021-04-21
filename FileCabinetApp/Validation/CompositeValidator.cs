using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Composit validator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Array of validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = new List<IRecordValidator>(validators);
        }

        /// <summary>
        /// FileCabinetRecord validator.
        /// </summary>
        /// <param name="parameters">Record.</param>
        /// <returns>Whether the parameter meets the specified criteria.</returns>
        public bool ValidateParameters(FileCabinetRecord parameters)
        {
            foreach (var validator in this.validators)
            {
                if (!validator.ValidateParameters(parameters))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
