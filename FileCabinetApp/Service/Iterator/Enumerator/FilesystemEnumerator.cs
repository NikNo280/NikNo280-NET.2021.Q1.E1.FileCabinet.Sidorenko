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
        }

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

        public void Dispose()
        {
        }

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

        public void Reset()
        {
            this.currentPosition = -1;
        }

        private FileCabinetRecord LazyInit()
        {
            return this.filesystemService.GetRecordByPosition(this.list[this.currentPosition]);
        }
    }
}
