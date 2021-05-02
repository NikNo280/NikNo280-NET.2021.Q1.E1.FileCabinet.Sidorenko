using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Service.Iterator.Enumerator;

namespace FileCabinetApp.Service.Iterator.Enumerable
{
    /// <summary>
    /// Enumerable of FileCabinetFilesystemService.
    /// </summary>
    public class FilesystemEnumerable : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetFilesystemService filesystemService;
        private List<long> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesystemEnumerable"/> class.
        /// </summary>
        /// <param name="filesystemService">FileCabinetFilesystemService.</param>
        /// <param name="list">List of positions of records in the file.</param>
        public FilesystemEnumerable(FileCabinetFilesystemService filesystemService, List<long> list)
        {
            this.filesystemService = filesystemService;
            this.list = list;
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var position in this.list)
            {
                yield return this.filesystemService.GetRecordByPosition(position);
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