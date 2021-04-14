using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetGenerator.RandomRecord
{
    public class DefaultRandomRecord : IRandomRecord
    {
        public static Random rnd = new Random();

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

        public DateTime GetRandomDateOfBirth()
        {
            DateTime start = new DateTime(1950, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rnd.Next(range));
        }

        public short GetRandomAge()
        {
            return (short)rnd.Next(1, 111);
        }

        public decimal GetRandomSalary()
        {
            return (decimal)rnd.NextDouble() + rnd.Next(1, int.MaxValue);
        }

        public char GetRandomGender()
        {
            return rnd.Next(0, 2) switch
            {
                0 => 'M',
                1 => 'W'
            };
        }
    }
}
