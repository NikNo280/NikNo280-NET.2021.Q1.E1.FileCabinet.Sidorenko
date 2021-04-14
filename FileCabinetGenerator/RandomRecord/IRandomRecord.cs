using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetGenerator.RandomRecord
{
    public interface IRandomRecord
    {
        public string GetRandomName();

        public DateTime GetRandomDateOfBirth();

        public short GetRandomAge();

        public decimal GetRandomSalary();

        public char GetRandomGender();
    }
}
