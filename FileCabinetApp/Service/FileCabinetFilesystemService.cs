using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FileCabinetApp.Service.Iterator;
using FileCabinetApp.Service.Iterator.Enumerable;

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

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Incorrect prarameters");
            }

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

            if (!this.recordValidator.ValidateParameters(record))
            {
                throw new ArgumentException("Incorrect prarameters");
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
        /// Insert record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void InsertRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} is null");
            }

            if (this.IsRecordById(record.Id))
            {
                this.EditRecord(record);
            }
            else
            {
                this.CreateRecord(record);
            }
        }

        /// <summary>
        /// Delete records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record.</param>
        /// <returns>Function execution result.</returns>
        public string DeleteRecords(PropertyInfo[] properties, FileCabinetRecord record)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            const string ZeroRecordsToDelete = "No record has been deleted";
            var firstNameProperty = typeof(FileCabinetRecord).GetProperty("FirstName");
            var lastNameProperty = typeof(FileCabinetRecord).GetProperty("LastName");
            var dateOfBirthProperty = typeof(FileCabinetRecord).GetProperty("DateOfBirth");
            var removeRecordId = new List<int>();
            if (properties.Contains(firstNameProperty))
            {
                string recordFirstNameValue = (string)firstNameProperty.GetValue(record);
                if (this.firstNameDictionary.ContainsKey(recordFirstNameValue.ToUpperInvariant()))
                {
                    foreach (var porsitionRecord in this.firstNameDictionary[recordFirstNameValue.ToUpperInvariant()])
                    {
                        var deleteRecord = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, deleteRecord, properties))
                        {
                            removeRecordId.Add(deleteRecord.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else if (properties.Contains(lastNameProperty))
            {
                string recordLastNameValue = (string)lastNameProperty.GetValue(record);
                if (this.lastNameDictionary.ContainsKey(recordLastNameValue.ToUpperInvariant()))
                {
                    foreach (var porsitionRecord in this.lastNameDictionary[recordLastNameValue.ToUpperInvariant()])
                    {
                        var deleteRecord = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, deleteRecord, properties))
                        {
                            removeRecordId.Add(deleteRecord.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else if (properties.Contains(dateOfBirthProperty))
            {
                DateTime recordDateOfBirthValue = (DateTime)dateOfBirthProperty.GetValue(record);
                if (this.dateOfBirthDictionary.ContainsKey(recordDateOfBirthValue))
                {
                    foreach (var porsitionRecord in this.dateOfBirthDictionary[recordDateOfBirthValue])
                    {
                        var deleteRecord = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, deleteRecord, properties))
                        {
                            removeRecordId.Add(deleteRecord.Id);
                        }
                    }
                }
                else
                {
                    return ZeroRecordsToDelete;
                }
            }
            else
            {
                int numBytesToRead = 0;
                while (numBytesToRead < this.fileStream.Length)
                {
                    var deleteRecord = this.GetRecordByPosition(numBytesToRead);
                    if (RecordEqualsByProperties(record, deleteRecord, properties))
                    {
                        removeRecordId.Add(deleteRecord.Id);
                    }

                    numBytesToRead += RecordSize;
                }
            }

            var stringBuilder = new StringBuilder();
            if (removeRecordId.Count == 0)
            {
                return ZeroRecordsToDelete;
            }
            else if (removeRecordId.Count == 1)
            {
                stringBuilder.Append("Record ");
            }
            else
            {
                stringBuilder.Append("Records ");
            }

            foreach (var id in removeRecordId)
            {
                stringBuilder.Append($"#{id}, ");
                this.Remove(id);
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 1);
            if (removeRecordId.Count == 1)
            {
                stringBuilder.Append("is deleted.");
            }
            else
            {
                stringBuilder.Append("are deleted.");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Update records.
        /// </summary>
        /// <param name="updateProperties">Properties to update.</param>
        /// <param name="updateRecord">Update record.</param>
        /// <param name="searchProperties">Properties to search.</param>
        /// <param name="searchRecord">Search record.</param>
        public void UpdateRecords(PropertyInfo[] updateProperties, FileCabinetRecord updateRecord, PropertyInfo[] searchProperties, FileCabinetRecord searchRecord)
        {
            if (updateProperties is null)
            {
                throw new ArgumentNullException(nameof(updateProperties));
            }

            if (updateRecord is null)
            {
                throw new ArgumentNullException(nameof(updateRecord));
            }

            if (searchProperties is null)
            {
                throw new ArgumentNullException(nameof(searchProperties));
            }

            if (searchRecord is null)
            {
                throw new ArgumentNullException(nameof(searchRecord));
            }

            var firstNameProperty = typeof(FileCabinetRecord).GetProperty("FirstName");
            var lastNameProperty = typeof(FileCabinetRecord).GetProperty("LastName");
            var dateOfBirthProperty = typeof(FileCabinetRecord).GetProperty("DateOfBirth");
            if (searchProperties.Contains(firstNameProperty))
            {
                string recordFirstNameValue = (string)firstNameProperty.GetValue(searchRecord);
                if (this.firstNameDictionary.ContainsKey(recordFirstNameValue.ToUpperInvariant()))
                {
                    var collection = this.firstNameDictionary[recordFirstNameValue.ToUpperInvariant()].ToArray();
                    foreach (var porsitionRecord in collection)
                    {
                        var record = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
                        {
                            foreach (var updateProperty in updateProperties)
                            {
                                updateProperty.SetValue(record, updateProperty.GetValue(updateRecord));
                            }

                            this.EditRecord(record);
                        }
                    }
                }
            }
            else if (searchProperties.Contains(lastNameProperty))
            {
                string recordLastNameValue = (string)lastNameProperty.GetValue(searchRecord);
                if (this.lastNameDictionary.ContainsKey(recordLastNameValue.ToUpperInvariant()))
                {
                    var collection = this.lastNameDictionary[recordLastNameValue.ToUpperInvariant()].ToArray();
                    foreach (var porsitionRecord in collection)
                    {
                        var record = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
                        {
                            foreach (var updateProperty in updateProperties)
                            {
                                updateProperty.SetValue(record, updateProperty.GetValue(updateRecord));
                            }

                            this.EditRecord(record);
                        }
                    }
                }
            }
            else if (searchProperties.Contains(dateOfBirthProperty))
            {
                DateTime recordDateOfBirthValue = (DateTime)dateOfBirthProperty.GetValue(searchRecord);
                if (this.dateOfBirthDictionary.ContainsKey(recordDateOfBirthValue))
                {
                    var collection = this.dateOfBirthDictionary[recordDateOfBirthValue].ToArray();
                    foreach (var porsitionRecord in collection)
                    {
                        var record = this.GetRecordByPosition(porsitionRecord);
                        if (RecordEqualsByProperties(record, searchRecord, searchProperties))
                        {
                            foreach (var updateProperty in updateProperties)
                            {
                                updateProperty.SetValue(record, updateProperty.GetValue(updateRecord));
                            }

                            this.EditRecord(record);
                        }
                    }
                }
            }
            else
            {
                int numBytesToRead = 0;
                while (numBytesToRead < this.fileStream.Length)
                {
                    var record = this.GetRecordByPosition(numBytesToRead);
                    if (RecordEqualsByProperties(record, searchRecord, searchProperties))
                    {
                        foreach (var updateProperty in updateProperties)
                        {
                            updateProperty.SetValue(record, updateProperty.GetValue(updateRecord));
                        }

                        this.EditRecord(record);
                    }

                    numBytesToRead += RecordSize;
                }
            }
        }

        /// <summary>
        /// Select records.
        /// </summary>
        /// <param name="properties">Properties to search.</param>
        /// <param name="record">Record to search.</param>
        /// <returns>Record Iterator.</returns>
        public IEnumerable<FileCabinetRecord> SelectRecords(PropertyInfo[][] properties, FileCabinetRecord[] record)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record is null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            int numBytesToRead = 0;
            bool result;
            var records = new List<long>();
            while (numBytesToRead < this.fileStream.Length)
            {
                result = true;
                var tempRecord = this.GetRecordByPosition(numBytesToRead);
                if (tempRecord is null)
                {
                    numBytesToRead += RecordSize;
                    continue;
                }

                if (properties.Length == 0)
                {
                    records.Add(numBytesToRead);
                    numBytesToRead += RecordSize;
                    continue;
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    result = true;
                    foreach (var property in properties[i])
                    {
                        if (!property.GetValue(record[i]).Equals(property.GetValue(tempRecord)))
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        records.Add(numBytesToRead);
                        break;
                    }
                }

                numBytesToRead += RecordSize;
            }

            return new FilesystemEnumerable(this, records);
        }

        /// <summary>
        /// looking for an record with this ID exists.
        /// </summary>
        /// <param name="id">Records id.</param>
        /// <returns>Is there a record with such an identifier.</returns>
        public bool IsRecordById(int id)
        {
            this.fileStream.Position = 0;
            byte[] bytes = new byte[4];
            for (int i = 0; i < this.GetStat(); i++)
            {
                this.fileStream.Read(bytes, 0, bytes.Length);
                if (!this.IsDeleted(i) && BitConverter.ToInt32(bytes, 0) == id)
                {
                    this.fileStream.Position = 0;
                    return true;
                }

                this.fileStream.Position = (i + 1) * RecordSize;
            }

            this.fileStream.Position = 0;
            return false;
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
            bool isDeleted = BitConverter.ToBoolean(data, RecordSize - sizeof(bool));
            this.fileStream.Position = fileStreamPosition;
            if (isDeleted)
            {
                return null;
            }
            else
            {
                return record;
            }
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

        private static bool RecordEqualsByProperties(FileCabinetRecord record1, FileCabinetRecord record2, PropertyInfo[] properties)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (record1 is null || record2 is null)
            {
                return false;
            }

            bool result = true;
            foreach (var property in properties)
            {
                var value1 = property.GetValue(record1);
                var value2 = property.GetValue(record2);
                if (value1 is null && value2 is null)
                {
                    continue;
                }
                else if (value1 is null && !(value2 is null))
                {
                    result = false;
                    break;
                }
                else if (!(value1 is null) && value2 is null)
                {
                    result = false;
                    break;
                }
                else if (!property.GetValue(record1).Equals(property.GetValue(record2)))
                {
                    result = false;
                    break;
                }
            }

            return result;
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