using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validation
{
    /// <summary>
    /// Gender validator.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char[] genders;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="genders">Array of gender.</param>
        public GenderValidator(char[] genders)
        {
            if (genders is null)
            {
                throw new ArgumentNullException(nameof(genders));
            }

            this.genders = new char[genders.Length];
            for (int i = 0; i < genders.Length; i++)
            {
                this.genders[i] = char.ToUpperInvariant(genders[i]);
            }
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

            return this.genders.Contains(char.ToUpperInvariant(parameters.Gender));
        }
    }
}
