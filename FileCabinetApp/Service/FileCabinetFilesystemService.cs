using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private FileStream fileStream;
        private readonly IRecordValidator recordValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator.</param>
        public FileCabinetFilesystemService(IRecordValidator recordValidator, FileStream fileStream)
        {
            this.recordValidator = recordValidator;
            this.fileStream = fileStream;
        }

        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{record} is null or empty");
            }

            this.recordValidator.IsValid(record);
            this.fileStream.Position = this.fileStream.Length;
            int offset = 0;
            byte[] bytes = BitConverter.GetBytes(record.Id);
            this.fileStream.Write(bytes, offset, bytes.Length);

            char[] firstName = new char[120];
            Array.Copy(record.FirstName.ToArray(), firstName, record.FirstName.Length);
            this.fileStream.Write(Encoding.UTF8.GetBytes(firstName), offset, Encoding.UTF8.GetByteCount(firstName));

            char[] lastName = new char[120];
            Array.Copy(record.LastName.ToArray(), lastName, record.LastName.Length);
            this.fileStream.Write(Encoding.UTF8.GetBytes(lastName), offset, Encoding.UTF8.GetByteCount(lastName));

            bytes = BitConverter.GetBytes(record.DateOfBirth.Year);
            this.fileStream.Write(bytes, offset, bytes.Length);
            bytes = BitConverter.GetBytes(record.DateOfBirth.Month);
            this.fileStream.Write(bytes, offset, bytes.Length);
            bytes = BitConverter.GetBytes(record.DateOfBirth.Day);
            this.fileStream.Write(bytes, offset, bytes.Length);

            bytes = BitConverter.GetBytes(record.Age);
            this.fileStream.Write(bytes, offset, bytes.Length);

            bytes = BitConverter.GetBytes(Convert.ToDouble(record.Salary));
            this.fileStream.Write(bytes, offset, bytes.Length);

            bytes = BitConverter.GetBytes(record.Gender);
            this.fileStream.Write(bytes, offset, bytes.Length);

            this.fileStream.Flush();
            return 0;
        }

        public void EditRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public int GetStat()
        {
            return (int)this.fileStream.Length / 268;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
