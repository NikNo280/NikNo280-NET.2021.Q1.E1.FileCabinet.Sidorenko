using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Iterator.Enumerator
{
    /// <summary>
    /// Lazy iterator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemEnumerator : IEnumerator<FileCabinetRecord>
    {
        private FileCabinetFilesystemService filesystemService;
        private List<long> list;
        private int currentPosition;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerator"/> class.
        /// </summary>
        /// <param name="filesystemService">FileCabinetFilesystemService.</param>
        /// <param name="list">List with item positions.</param>
        public FilesystemEnumerator(FileCabinetFilesystemService filesystemService, List<long> list)
        {
            this.filesystemService = filesystemService;
            this.list = list;
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
                if (this.currentPosition == -1 || this.currentPosition >= this.list.Count)
                {
                    throw new InvalidOperationException();
                }

                return this.LazyInit();
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
                if (this.currentPosition == -1 || this.currentPosition >= this.list.Count)
                {
                    throw new InvalidOperationException();
                }

                return this.LazyInit();
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
            if (this.currentPosition < this.list.Count - 1)
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

        private FileCabinetRecord LazyInit()
        {
            return this.filesystemService.GetRecordByPosition(this.list[this.currentPosition]);
        }
    }
}
