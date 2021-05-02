using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Service.Iterator.Enumerator;

namespace FileCabinetApp.Service.Iterator.Enumerable
{
    /// <summary>
    /// Enumerable of FileCabinetMemoryService.
    /// </summary>
    public class MemoryEnumerable : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerable"/> class.
        /// </summary>
        /// <param name="records">Array of records.</param>
        public MemoryEnumerable(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var record in this.records)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
