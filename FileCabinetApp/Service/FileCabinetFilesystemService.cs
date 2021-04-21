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
        private const int RecordSize = 277;
        private const int NameSize = 120;
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

            this.recordValidator.ValidateParameters(record);
            this.fileStream.Position = this.fileStream.Length;
            int offset = 0;
            byte[] bytes = BitConverter.GetBytes(record.Id);
            this.fileStream.Write(bytes, offset, bytes.Length);

            char[] firstName = new char[NameSize];
            Array.Copy(record.FirstName.ToArray(), firstName, record.FirstName.Length);
            this.fileStream.Write(Encoding.UTF8.GetBytes(firstName), offset, Encoding.UTF8.GetByteCount(firstName));

            char[] lastName = new char[NameSize];
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

            /*
            int[] bits = decimal.GetBits(record.Salary);
            foreach (var bit in bits)
            {

            }
            */
            bytes = BitConverter.GetBytes(Convert.ToDouble(record.Salary));
            this.fileStream.Write(bytes, offset, bytes.Length);

            bytes = BitConverter.GetBytes(record.Gender);
            this.fileStream.Write(bytes, offset, bytes.Length);

            bytes = BitConverter.GetBytes(false);
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

            this.fileStream.Position = 0;
            this.recordValidator.ValidateParameters(record);
            int offset = 0;
            byte[] bytes = new byte[4];
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Read(bytes, offset, bytes.Length);
                if (!this.IsDeleted(i) && BitConverter.ToInt32(bytes, 0) == record.Id)
                {
                    char[] firstName = new char[NameSize];
                    Array.Copy(record.FirstName.ToArray(), firstName, record.FirstName.Length);
                    this.fileStream.Write(Encoding.UTF8.GetBytes(firstName), offset, Encoding.UTF8.GetByteCount(firstName));

                    char[] lastName = new char[NameSize];
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
                    return;
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
            this.fileStream.Position = 0;
            int numBytesToRead = (int)this.fileStream.Length;
            int offset = 0;
            var records = new List<FileCabinetRecord>();
            byte[] bytes = new byte[RecordSize];
            while (numBytesToRead > 0)
            {
                this.fileStream.Read(bytes, offset, bytes.Length);
                var record = new FileCabinetRecord
                {
                    Id = BitConverter.ToInt32(bytes, 0),
                    FirstName = Encoding.UTF8.GetString(bytes, 4, NameSize).Trim(new char[] { '\0' }),
                    LastName = Encoding.UTF8.GetString(bytes, 124, NameSize).Trim(new char[] { '\0' }),
                    DateOfBirth = new DateTime(BitConverter.ToInt32(bytes, 244), BitConverter.ToInt32(bytes, 248), BitConverter.ToInt32(bytes, 252)),
                    Age = BitConverter.ToInt16(bytes, 256),
                    Salary = Convert.ToDecimal(BitConverter.ToDouble(bytes, 258)),
                    Gender = BitConverter.ToChar(bytes, 266),
                };

                if (!BitConverter.ToBoolean(bytes, 268))
                {
                    records.Add(record);
                }

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
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null or empty");
            }

            int offset = 0;
            byte[] bytes = new byte[NameSize];
            var records = new List<FileCabinetRecord>();
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Position += 4;
                this.fileStream.Read(bytes, offset, bytes.Length);
                if (!this.IsDeleted(i) && Encoding.UTF8.GetString(bytes, 0, NameSize).Trim(new char[] { '\0' }).ToUpperInvariant().Equals(firstName.ToUpperInvariant()))
                {
                    this.fileStream.Position = i * RecordSize;
                    byte[] data = new byte[RecordSize];
                    this.fileStream.Read(data, offset, data.Length);
                    var record = new FileCabinetRecord
                    {
                        Id = BitConverter.ToInt32(data, 0),
                        FirstName = Encoding.UTF8.GetString(data, 4, NameSize).Trim(new char[] { '\0' }),
                        LastName = Encoding.UTF8.GetString(data, 124, NameSize).Trim(new char[] { '\0' }),
                        DateOfBirth = new DateTime(BitConverter.ToInt32(data, 244), BitConverter.ToInt32(data, 248), BitConverter.ToInt32(data, 252)),
                        Age = BitConverter.ToInt16(data, 256),
                        Salary = Convert.ToDecimal(BitConverter.ToDouble(data, 258)),
                        Gender = BitConverter.ToChar(data, 266),
                    };
                    records.Add(record);
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null or empty");
            }

            int offset = 0;
            byte[] bytes = new byte[NameSize];
            var records = new List<FileCabinetRecord>();
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Position += 124;
                this.fileStream.Read(bytes, offset, bytes.Length);
                if (!this.IsDeleted(i) && Encoding.UTF8.GetString(bytes, 0, NameSize).Trim(new char[] { '\0' }).ToUpperInvariant().Equals(lastName.ToUpperInvariant()))
                {
                    this.fileStream.Position = i * RecordSize;
                    byte[] data = new byte[RecordSize];
                    this.fileStream.Read(data, offset, data.Length);
                    var record = new FileCabinetRecord
                    {
                        Id = BitConverter.ToInt32(data, 0),
                        FirstName = Encoding.UTF8.GetString(data, 4, NameSize).Trim(new char[] { '\0' }),
                        LastName = Encoding.UTF8.GetString(data, 124, NameSize).Trim(new char[] { '\0' }),
                        DateOfBirth = new DateTime(BitConverter.ToInt32(data, 244), BitConverter.ToInt32(data, 248), BitConverter.ToInt32(data, 252)),
                        Age = BitConverter.ToInt16(data, 256),
                        Salary = Convert.ToDecimal(BitConverter.ToDouble(data, 258)),
                        Gender = BitConverter.ToChar(data, 266),
                    };
                    records.Add(record);
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            return new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            if (string.IsNullOrWhiteSpace(dateofbirth))
            {
                throw new ArgumentNullException($"{nameof(dateofbirth)} is null or empty");
            }

            DateTime searchDateTime;
            var result = DateTime.TryParseExact(dateofbirth, "yyyy-MMM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateTime);
            if (result)
            {
                int offset = 0;
                byte[] bytes = new byte[4];
                var records = new List<FileCabinetRecord>();
                for (int i = 0; i < this.GetStat(); i++)
                {
                    this.fileStream.Position += 244;
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int year = BitConverter.ToInt32(bytes, 0);
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int month = BitConverter.ToInt32(bytes, 0);
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int day = BitConverter.ToInt32(bytes, 0);
                    if (!this.IsDeleted(i) && new DateTime(year, month, day).Equals(searchDateTime))
                    {
                        this.fileStream.Position = i * RecordSize;
                        byte[] data = new byte[RecordSize];
                        this.fileStream.Read(data, offset, data.Length);
                        var record = new FileCabinetRecord
                        {
                            Id = BitConverter.ToInt32(data, 0),
                            FirstName = Encoding.UTF8.GetString(data, 4, NameSize).Trim(new char[] { '\0' }),
                            LastName = Encoding.UTF8.GetString(data, 124, NameSize).Trim(new char[] { '\0' }),
                            DateOfBirth = new DateTime(BitConverter.ToInt32(data, 244), BitConverter.ToInt32(data, 248), BitConverter.ToInt32(data, 252)),
                            Age = BitConverter.ToInt16(data, 256),
                            Salary = Convert.ToDecimal(BitConverter.ToDouble(data, 258)),
                            Gender = BitConverter.ToChar(data, 266),
                        };
                        records.Add(record);
                    }

                    this.fileStream.Position = (i + 1) * RecordSize;
                }

                this.fileStream.Position = 0;
                return new ReadOnlyCollection<FileCabinetRecord>(records);
            }
            else
            {
                return new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>());
            }
        }

        /// <summary>
        /// Generate new FileCabinetRecord snapshot.
        /// </summary>
        /// <returns>new FileCabinetRecord snapshot.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
        }

        /// <summary>
        /// Restores data from snapshot.
        /// </summary>
        /// <param name="snapshot">FileCabinet snapshot.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} is null");
            }

            Dictionary<int, FileCabinetRecord> id_dictionary = new Dictionary<int, FileCabinetRecord>();

            foreach (var record in this.GetRecords())
            {
                id_dictionary.Add(record.Id, record);
            }

            foreach (var record in snapshot.Records)
            {
                if (id_dictionary.ContainsKey(record.Id))
                {
                    this.EditRecord(record);
                }
                else
                {
                    this.CreateRecord(record);
                }
            }
        }

        /// <summary>
        /// Gets last index of records.
        /// </summary>
        /// <returns>Last index of records.</returns>
        public int GetLastIndex()
        {
            if (this.fileStream.Length == 0)
            {
                return 0;
            }
            else
            {
                return this.FindMaxId();
            }
        }

        /// <summary>
        /// /// Finds the maximum index of the records.
        /// </summary>
        /// <returns>The maximum index of the records.</returns>
        public int FindMaxId()
        {
            this.fileStream.Position = 0;
            int offset = 0;
            byte[] bytes = new byte[4];
            int max = int.MinValue;
            int temp;
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Read(bytes, offset, bytes.Length);
                temp = BitConverter.ToInt32(bytes, 0);
                if (!this.IsDeleted(i) && BitConverter.ToInt32(bytes, 0) > max)
                {
                    max = temp;
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            return max;
        }

        /// <summary>
        /// Removes records.
        /// </summary>
        /// <param name="id">Id record to delete.</param>
        /// <returns>Whether the entry has been deleted.</returns>
        public bool Remove(int id)
        {
            this.fileStream.Position = 0;
            int offset = 0;
            byte[] bytes = new byte[4];
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Read(bytes, offset, bytes.Length);
                int recordId = BitConverter.ToInt32(bytes, 0);
                bool isDeleted = this.IsDeleted(i);

                if (recordId == id && !isDeleted)
                {
                    this.fileStream.Position += RecordSize - sizeof(int) - sizeof(bool);
                    bytes = BitConverter.GetBytes(true);
                    this.fileStream.Write(bytes, offset, bytes.Length);
                    this.fileStream.Position = 0;
                    return true;
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            return false;
        }

        /// <summary>
        /// Defragments the data file.
        /// </summary>
        public void Purge()
        {
            long numBytesToRead = this.fileStream.Length;
            int offset = 0;
            byte[] bytes = new byte[RecordSize];
            using (FileStream file = File.Open("temp.db", FileMode.Create))
            {
                while (numBytesToRead > 0)
                {
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    if (!BitConverter.ToBoolean(bytes, 268))
                    {
                        file.Write(bytes, 0, bytes.Length);
                    }

                    numBytesToRead -= RecordSize;
                }

                this.fileStream.SetLength(0);

                numBytesToRead = file.Length;

                file.Position = 0;
                while (numBytesToRead > 0)
                {
                    file.Read(bytes, offset, bytes.Length);
                    this.fileStream.Write(bytes, 0, bytes.Length);
                    numBytesToRead -= RecordSize;
                }
            }

            this.fileStream.Position = 0;
            File.Delete("temp.db");
        }

        /// <summary>
        /// Gets number of records deleted.
        /// </summary>
        /// <returns>Number of records deleted.</returns>
        public int GetCountDeletedRecords()
        {
            int count = 0;
            for (int i = 0; i < this.GetStat(); i++)
            {
                if (this.IsDeleted(i))
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsDeleted(int index)
        {
            long tempPosition = this.fileStream.Position;
            byte[] isDeleted_bytes = new byte[1];
            this.fileStream.Position = ((index + 1) * RecordSize) - 1;
            this.fileStream.Read(isDeleted_bytes, 0, isDeleted_bytes.Length);
            this.fileStream.Position = tempPosition;
            return BitConverter.ToBoolean(isDeleted_bytes, 0);
        }
    }
}
