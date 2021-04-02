using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// The class provides static methods for conversion.
    /// </summary>
    public static class TypeConverter
    {
        /// <summary>
        /// Converts string to string.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and string.</returns>
        public static Tuple<bool, string, string> StringConverter(string source)
        {
            return new Tuple<bool, string, string>(true, source, source);
        }

        /// <summary>
        /// Converts string to int.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and int number.</returns>
        public static Tuple<bool, string, int> IntConverter(string source)
        {
            int value;
            var result = int.TryParse(source, out value);
            return new Tuple<bool, string, int>(result, "source not converted to int", value);
        }

        /// <summary>
        /// Converts string to decimal.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and decimal number.</returns>
        public static Tuple<bool, string, decimal> DecimalConverter(string source)
        {
            decimal value;
            var result = decimal.TryParse(source, out value);
            return new Tuple<bool, string, decimal>(result, "source not converted to decimal", value);
        }

        /// <summary>
        /// Converts string to short.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and short number.</returns>
        public static Tuple<bool, string, short> ShortConverter(string source)
        {
            short value;
            var result = short.TryParse(source, out value);
            return new Tuple<bool, string, short>(result, "source not converted to short", value);
        }

        /// <summary>
        /// Converts string to DateTime.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and DateTime.</returns>
        public static Tuple<bool, string, DateTime> DateTimeConverter(string source)
        {
            DateTime value;
            var result = DateTime.TryParseExact(source, "d", CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
            return new Tuple<bool, string, DateTime>(result, "source not converted to DateTime", value);
        }

        /// <summary>
        /// Converts string to char.
        /// </summary>
        /// <param name="source">Input string.</param>
        /// <returns>Conversion result, error message, and char.</returns>
        public static Tuple<bool, string, char> CharConverter(string source)
        {
            if (source is null)
            {
                return new Tuple<bool, string, char>(false, "source is null", ' ');
            }

            return new Tuple<bool, string, char>(true, "ok", char.ToUpper(source[0], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Converts string to T type.
        /// </summary>
        /// <typeparam name="T">Conversion type.</typeparam>
        /// <param name="converter">Conversion method.</param>
        /// <param name="validator">Validation method.</param>
        /// <returns>Conversion result from string from T.</returns>
        public static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            if (converter is null)
            {
                throw new ArgumentNullException($"{nameof(converter)} is null");
            }

            if (validator is null)
            {
                throw new ArgumentNullException($"{nameof(validator)} is null");
            }

            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
