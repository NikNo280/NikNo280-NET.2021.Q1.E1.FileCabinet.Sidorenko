using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Iterator.Enumerator
{
    /// <summary>
    /// Iterator for FileCabinetMemoryService.
    /// </summary>
    public class MemoryEnumerator : IEnumerator<FileCabinetRecord>
    {
        private FileCabinetRecord[] records;
        private int currentPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerator"/> class.
        /// </summary>
        /// <param name="list">List of records.</param>
        public MemoryEnumerator(FileCabinetRecord[] records)
        {
            this.records = records;
            this.currentPosition = -1;
        }

        public object Current
        {
            get
            {
                if (this.currentPosition == -1 || this.currentPosition >= this.records.Length)
                {
                    throw new InvalidOperationException();
                }

                return this.records[this.currentPosition];
            }
        }

        FileCabinetRecord IEnumerator<FileCabinetRecord>.Current
        {
            get
            {
                if (this.currentPosition == -1 || this.currentPosition >= this.records.Length)
                {
                    throw new InvalidOperationException();
                }

                return this.records[this.currentPosition];
            }
        }

        public void Dispose() 
        {
        }

        public bool MoveNext()
        {
            if (this.currentPosition < this.records.Length - 1)
            {
                this.currentPosition++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            this.currentPosition = -1;
        }
    }
}
