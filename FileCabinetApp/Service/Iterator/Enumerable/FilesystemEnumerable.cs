using System.Collections;
using System.Collections.Generic;
using FileCabinetApp.Service.Iterator.Enumerator;

namespace FileCabinetApp.Service.Iterator.Enumerable
{
    public class FilesystemEnumerable : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetFilesystemService filesystemService;
        private List<long> list;

        public FilesystemEnumerable(FileCabinetFilesystemService filesystemService, List<long> list)
        {
            this.filesystemService = filesystemService;
            this.list = list;
        }

        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new FilesystemEnumerator(this.filesystemService, this.list);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}