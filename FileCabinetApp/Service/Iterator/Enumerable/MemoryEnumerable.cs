using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Service.Iterator.Enumerator;

namespace FileCabinetApp.Service.Iterator.Enumerable
{
    public class MemoryEnumerable : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetRecord[] records;

        public MemoryEnumerable(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new MemoryEnumerator(this.records);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
