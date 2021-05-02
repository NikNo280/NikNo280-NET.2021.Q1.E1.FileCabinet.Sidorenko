using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Service.Iterator
{
    /// <summary>
    /// Lazy iterator for FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemIterator : IRecordIterator
    {
        private FileCabinetFilesystemService filesystemService;
        private List<long> list;
        private int currentPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemIterator"/> class.
        /// </summary>
        /// <param name="filesystemService">FileCabinetFilesystemService.</param>
        /// <param name="list">List with item positions.</param>
        public FilesystemIterator(FileCabinetFilesystemService filesystemService, List<long> list)
        {
            this.filesystemService = filesystemService;
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
                return this.LazyInit();
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

        private FileCabinetRecord LazyInit()
        {
            return this.filesystemService.GetRecordByPosition(this.list[this.currentPosition]);
        }
    }
}
