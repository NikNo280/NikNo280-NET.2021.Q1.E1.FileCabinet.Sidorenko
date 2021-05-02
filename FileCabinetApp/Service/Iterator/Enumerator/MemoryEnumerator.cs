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
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEnumerator"/> class.
        /// </summary>
        /// <param name="records">Array of records.</param>
        public MemoryEnumerator(FileCabinetRecord[] records)
        {
            this.records = records;
            this.currentPosition = -1;
            this.disposed = false;
        }

        /// <summary>
        /// Gets current item.
        /// </summary>
        /// <value>
        /// Current item.
        /// </value>
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

        /// <summary>
        /// Gets current item.
        /// </summary>
        /// <value>
        /// Current item.
        /// </value>
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

        /// <summary>
        /// Dispose object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Move to the next item.
        /// </summary>
        /// <returns>Returns whether there is a next item.</returns>
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

        /// <summary>
        /// Reset current position.
        /// </summary>
        public void Reset()
        {
            this.currentPosition = -1;
        }

        /// <summary>
        /// Dispose object.
        /// </summary>
        /// <param name="disposing">Whether the object has been deleted.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }

                this.disposed = true;
            }
        }
    }
}
