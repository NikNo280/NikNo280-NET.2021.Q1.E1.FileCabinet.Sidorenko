using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetGenerator.RandomRecord
{
    /// <summary>
    /// This class provides functions to randomly generate records.
    /// </summary>
    public class DefaultRandomRecord : IRandomRecord
    {
        private static Random rnd = new Random();

        /// <summary>
        /// Gets random name.
        /// </summary>
        /// <returns>Random name.</returns>
        public string GetRandomName()
        {
            char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            int size = rnd.Next(1, 60);
            var name = new StringBuilder();
            name.Append(char.ToUpper(letters[rnd.Next(0, letters.Length)], CultureInfo.InvariantCulture));
            for (int i = 0; i < size; i++)
            {
                name.Append(letters[rnd.Next(0, letters.Length)]);
            }

            return name.ToString();
        }

        /// <summary>
        /// Gets random date of birth.
        /// </summary>
        /// <returns>Random date of birth.</returns>
        public DateTime GetRandomDateOfBirth()
        {
            DateTime start = new DateTime(1950, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rnd.Next(range));
        }

        /// <summary>
        /// Gets random age.
        /// </summary>
        /// <returns>Random age.</returns>
        public short GetRandomAge()
        {
            return (short)rnd.Next(1, 111);
        }

        /// <summary>
        /// Gets random salary.
        /// </summary>
        /// <returns>Random salary.</returns>
        public decimal GetRandomSalary()
        {
            return (decimal)rnd.NextDouble() + rnd.Next(1, int.MaxValue);
        }

        /// <summary>
        /// Gets random gender.
        /// </summary>
        /// <returns>Random gender.</returns>
        public char GetRandomGender()
        {
            return rnd.Next(0, 2) switch
            {
                0 => 'M',
                1 => 'W',
                _ => 'M',
            };
        }
    }
}
