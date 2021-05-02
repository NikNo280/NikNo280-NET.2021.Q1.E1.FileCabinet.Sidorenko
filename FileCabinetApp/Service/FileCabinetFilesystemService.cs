using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Service.Iterator;

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
        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="recordValidator">Validator.</param>
        /// <param name="fileStream">File stream.</param>
        public FileCabinetFilesystemService(IRecordValidator recordValidator, FileStream fileStream)
        {
            this.recordValidator = recordValidator;
            this.fileStream = fileStream;
            this.RecalculationIndices();
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

            AddToDict(record.FirstName.ToUpperInvariant(), this.firstNameDictionary, this.fileStream.Position);
            AddToDict(record.LastName.ToUpperInvariant(), this.lastNameDictionary, this.fileStream.Position);
            AddToDict(record.DateOfBirth, this.dateOfBirthDictionary, this.fileStream.Position);

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

            int[] bits = decimal.GetBits(record.Salary);
            foreach (var bit in bits)
            {
                bytes = BitConverter.GetBytes(bit);
                this.fileStream.Write(bytes, offset, bytes.Length);
            }

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
                    long position = this.fileStream.Position;
                    bytes = new byte[120];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    string firstNameToDict = Encoding.UTF8.GetString(bytes, offset, NameSize).Trim(new char[] { '\0' });
                    bytes = new byte[120];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    string lastNameToDict = Encoding.UTF8.GetString(bytes, offset, NameSize).Trim(new char[] { '\0' });
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int yearToDict = BitConverter.ToInt32(bytes, 0);
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int monthToDict = BitConverter.ToInt32(bytes, 0);
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int dayToDict = BitConverter.ToInt32(bytes, 0);
                    DateTime dateOfBirthToDict = new DateTime(yearToDict, monthToDict, dayToDict);
                    UpdateDict(this.firstNameDictionary, firstNameToDict.ToUpperInvariant(), record.FirstName.ToUpperInvariant(), i * RecordSize);
                    UpdateDict(this.lastNameDictionary, lastNameToDict.ToUpperInvariant(), record.LastName.ToUpperInvariant(), i * RecordSize);
                    UpdateDict(this.dateOfBirthDictionary, dateOfBirthToDict, record.DateOfBirth, i * RecordSize);
                    this.fileStream.Position = position;

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

                    int[] bits = decimal.GetBits(record.Salary);
                    foreach (var bit in bits)
                    {
                        bytes = BitConverter.GetBytes(bit);
                        this.fileStream.Write(bytes, offset, bytes.Length);
                    }

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
                    Gender = BitConverter.ToChar(bytes, 274),
                };

                int[] bits = new int[4];
                int position = 258;
                for (int i = 0; i < bits.Length; i++)
                {
                    bits[i] = BitConverter.ToInt32(bytes, position);
                    position += sizeof(int);
                }

                record.Salary = new decimal(bits);

                if (!BitConverter.ToBoolean(bytes, RecordSize - sizeof(bool)))
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
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException($"{nameof(firstName)} is null or empty");
            }

            firstName = firstName.ToUpperInvariant();
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                return new FilesystemIterator(this, this.firstNameDictionary[firstName]);
            }

            return new FilesystemIterator(this, new List<long>());
        }

        /// <summary>
        /// Find records by last name.
        /// </summary>
        /// <param name="lastName">Users last name.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException($"{nameof(lastName)} is null or empty");
            }

            lastName = lastName.ToUpperInvariant();
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                return new FilesystemIterator(this, this.lastNameDictionary[lastName]);
            }

            return new FilesystemIterator(this, new List<long>());
        }

        /// <summary>
        /// Find records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Users date of birth.</param>
        /// <returns>Record Iterator.</returns>
        public IRecordIterator FindByDateOfBirth(string dateofbirth)
        {
            if (string.IsNullOrWhiteSpace(dateofbirth))
            {
                throw new ArgumentNullException($"{nameof(dateofbirth)} is null or empty");
            }

            DateTime dateTime;
            bool result = DateTime.TryParseExact(dateofbirth, "yyyy-MMM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            if (result && this.dateOfBirthDictionary.ContainsKey(dateTime))
            {
                return new FilesystemIterator(this, this.dateOfBirthDictionary[dateTime]);
            }

            return new FilesystemIterator(this, new List<long>());
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

            this.RecalculationIndices();
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

                    this.fileStream.Position = (RecordSize * i) + 4;
                    bytes = new byte[120];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    string firstNameToDict = Encoding.UTF8.GetString(bytes, offset, NameSize).Trim(new char[] { '\0' });
                    bytes = new byte[120];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    string lastNameToDict = Encoding.UTF8.GetString(bytes, offset, NameSize).Trim(new char[] { '\0' });
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int yearToDict = BitConverter.ToInt32(bytes, 0);
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int monthToDict = BitConverter.ToInt32(bytes, 0);
                    bytes = new byte[4];
                    this.fileStream.Read(bytes, offset, bytes.Length);
                    int dayToDict = BitConverter.ToInt32(bytes, 0);
                    DateTime dateOfBirthToDict = new DateTime(yearToDict, monthToDict, dayToDict);
                    RemovePositionInDict(this.firstNameDictionary, i * RecordSize, firstNameToDict.ToUpperInvariant());
                    RemovePositionInDict(this.lastNameDictionary, i * RecordSize, lastNameToDict.ToUpperInvariant());
                    RemovePositionInDict(this.dateOfBirthDictionary, i * RecordSize, dateOfBirthToDict);
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
                    if (!BitConverter.ToBoolean(bytes, RecordSize - sizeof(bool)))
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
            this.RecalculationIndices();
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

        /// <summary>
        /// Gets record by position in file.
        /// </summary>
        /// <param name="position">Position in file.</param>
        /// <returns>Record.</returns>
        public FileCabinetRecord GetRecordByPosition(long position)
        {
            long fileStreamPosition = this.fileStream.Position;
            this.fileStream.Position = position;
            int offset = 0;
            byte[] data = new byte[RecordSize];
            this.fileStream.Read(data, offset, data.Length);
            var record = new FileCabinetRecord
            {
                Id = BitConverter.ToInt32(data, 0),
                FirstName = Encoding.UTF8.GetString(data, 4, NameSize).Trim(new char[] { '\0' }),
                LastName = Encoding.UTF8.GetString(data, 124, NameSize).Trim(new char[] { '\0' }),
                DateOfBirth = new DateTime(BitConverter.ToInt32(data, 244), BitConverter.ToInt32(data, 248), BitConverter.ToInt32(data, 252)),
                Age = BitConverter.ToInt16(data, 256),
                Gender = BitConverter.ToChar(data, 274),
            };

            int[] bits = new int[4];
            int positionTemp = 258;
            for (int j = 0; j < bits.Length; j++)
            {
                bits[j] = BitConverter.ToInt32(data, positionTemp);
                positionTemp += sizeof(int);
            }

            record.Salary = new decimal(bits);
            this.fileStream.Position = fileStreamPosition;
            return record;
        }

        /// <summary>
        /// /// Finds the maximum index of the records.
        /// </summary>
        /// <returns>The maximum index of the records.</returns>
        protected int FindMaxId()
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

        private static void RemovePositionInDict<T>(Dictionary<T, List<long>> dictionary, long position, T key)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException($"{nameof(dictionary)} is null");
            }

            if (dictionary.ContainsKey(key))
            {
                var fileCabinetRecords = dictionary[key];
                if (fileCabinetRecords.Contains(position))
                {
                    if (fileCabinetRecords.Count == 1)
                    {
                        dictionary.Remove(key);
                    }
                    else
                    {
                        fileCabinetRecords.Remove(position);
                    }
                }
            }
        }

        private static ReadOnlyCollection<long> GetArrayFromDict<T>(T source, Dictionary<T, List<long>> dictionary)
        {
            if (dictionary.ContainsKey(source))
            {
                return new ReadOnlyCollection<long>(dictionary[source]);
            }
            else
            {
                return new ReadOnlyCollection<long>(new List<long>());
            }
        }

        private static void AddToDict<T>(T key, Dictionary<T, List<long>> dictionary, long position)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(position);
            }
            else
            {
                dictionary.Add(key, new List<long>() { position });
            }
        }

        private static void UpdateDict<T>(Dictionary<T, List<long>> dictionary, T oldKey, T newKey, long position)
        {
            dictionary[oldKey].Remove(dictionary[oldKey].Where(item => item == position).First());
            AddToDict(newKey, dictionary, position);
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

        private FileCabinetRecord GetRecordById(int i)
        {
            this.fileStream.Position = i * RecordSize;
            int offset = 0;
            byte[] data = new byte[RecordSize];
            this.fileStream.Read(data, offset, data.Length);
            var record = new FileCabinetRecord
            {
                Id = BitConverter.ToInt32(data, 0),
                FirstName = Encoding.UTF8.GetString(data, 4, NameSize).Trim(new char[] { '\0' }),
                LastName = Encoding.UTF8.GetString(data, 124, NameSize).Trim(new char[] { '\0' }),
                DateOfBirth = new DateTime(BitConverter.ToInt32(data, 244), BitConverter.ToInt32(data, 248), BitConverter.ToInt32(data, 252)),
                Age = BitConverter.ToInt16(data, 256),
                Gender = BitConverter.ToChar(data, 274),
            };

            int[] bits = new int[4];
            int position = 258;
            for (int j = 0; j < bits.Length; j++)
            {
                bits[j] = BitConverter.ToInt32(data, position);
                position += sizeof(int);
            }

            record.Salary = new decimal(bits);
            return record;
        }

        private void RecalculationIndices()
        {
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();
            for (int i = 0; i < this.GetStat(); i++)
            {
                if (!this.IsDeleted(i))
                {
                    byte[] bytes = new byte[(NameSize * 2) + (sizeof(int) * 3)];
                    this.fileStream.Position = (i * RecordSize) + sizeof(int);
                    this.fileStream.Read(bytes, 0, bytes.Length);
                    string firstName = Encoding.UTF8.GetString(bytes, 0, NameSize).Trim(new char[] { '\0' });
                    string lastName = Encoding.UTF8.GetString(bytes, NameSize, NameSize).Trim(new char[] { '\0' });
                    int year = BitConverter.ToInt32(bytes, NameSize * 2);
                    int month = BitConverter.ToInt32(bytes, (NameSize * 2) + sizeof(int));
                    int day = BitConverter.ToInt32(bytes, (NameSize * 2) + (sizeof(int) * 2));
                    DateTime dateOfBirth = new DateTime(year, month, day);
                    AddToDict(firstName.ToUpperInvariant(), this.firstNameDictionary, i * RecordSize);
                    AddToDict(lastName.ToUpperInvariant(), this.lastNameDictionary, i * RecordSize);
                    AddToDict(dateOfBirth, this.dateOfBirthDictionary, i * RecordSize);
                    this.fileStream.Read(bytes, 0, bytes.Length);
                }
            }
        }
    }
}