using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Iterator
{
    /// <summary>
    /// Iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryIterator : IRecordIterator
    {
        private List<FileCabinetRecord> list;
        private int currentPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="list">List of records.</param>
        public MemoryIterator(List<FileCabinetRecord> list)
        {
            this.list = list;
            this.currentPosition = -1;
        }

        /// <summary>
        /// Gets next record.
        /// </summary>
        /// <returns>Record.</returns>
        public FileCabinetRecord GetNext()
        {
            if (this.HasMore())
            {
                this.currentPosition++;
                return this.list[this.currentPosition];
            }

            return new FileCabinetRecord();
        }

        /// <summary>
        /// Checks if the records have finished.
        /// </summary>
        /// <returns>Boolean value whether records have run out.</returns>
        public bool HasMore()
        {
            return this.currentPosition < this.list.Count - 1;
        }
    }
}
