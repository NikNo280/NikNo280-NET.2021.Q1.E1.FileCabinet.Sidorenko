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
    /// <summary>
    /// The class representing functions for interacting with the record model.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 268;
        private readonly IRecordValidator recordValidator;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator.</param>
        /// <param name="fileStream">File stream.</param>
        public FileCabinetFilesystemService(IRecordValidator recordValidator, FileStream fileStream)
        {
            this.recordValidator = recordValidator;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Create new record.
        /// </summary>
        /// <param name="record">New record.</param>
        /// <returns>Returns new record id.</returns>
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
            this.fileStream.Position = 0;

            return record.Id;
        }

        /// <summary>
        /// Modify an existing record by id.
        /// </summary>
        /// <param name="record">New record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            this.recordValidator.IsValid(record);
            int offset = 0;
            byte[] bytes = new byte[4];
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Read(bytes, offset, bytes.Length);
                if (BitConverter.ToInt32(bytes, 0) == record.Id)
                {
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
                    this.fileStream.Position = 0;
                    break;
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            throw new ArgumentException($"#{record.Id} record is not found.");
        }

        /// <summary>
        /// Gets all records.
        /// </summary>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            int numBytesToRead = (int)this.fileStream.Length;
            int offset = 0;
            var records = new List<FileCabinetRecord>();
            while (numBytesToRead > 0)
            {
                byte[] bytes = new byte[RecordSize];
                this.fileStream.Read(bytes, offset, bytes.Length);
                var record = new FileCabinetRecord
                {
                    Id = BitConverter.ToInt32(bytes, 0),
                    FirstName = Encoding.UTF8.GetString(bytes, 4, 120).Trim(new char[] { '\0' }),
                    LastName = Encoding.UTF8.GetString(bytes, 124, 120).Trim(new char[] { '\0' }),
                    DateOfBirth = new DateTime(BitConverter.ToInt32(bytes, 244), BitConverter.ToInt32(bytes, 248), BitConverter.ToInt32(bytes, 252)),
                    Age = BitConverter.ToInt16(bytes, 256),
                    Salary = Convert.ToDecimal(BitConverter.ToDouble(bytes, 258)),
                    Gender = BitConverter.ToChar(bytes, 266),
                };
                records.Add(record);
                numBytesToRead -= RecordSize;
            }

            this.fileStream.Position = 0;
            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Gets count of records.
        /// </summary>
        /// <returns>Return count of records.</returns>
        public int GetStat()
        {
            return (int)this.fileStream.Length / RecordSize;
        }

        /// <summary>
        /// Find records by first name.
        /// </summary>
        /// <param name="firstName">Users first name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
