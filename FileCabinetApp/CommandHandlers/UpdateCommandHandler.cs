using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a update command hadler.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Command handler.
        /// </summary>
        /// <param name="appCommandRequest">Request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException($"{nameof(appCommandRequest)} is null");
            }

            if (string.Equals(appCommandRequest.Command, "update", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] updateFieldsNames, updateFieldsValues, searchFieldsNames, searchFieldsValues;
                bool result;
                var properties = typeof(FileCabinetRecord).GetProperties();
                (updateFieldsNames, updateFieldsValues, searchFieldsNames, searchFieldsValues, result) = UpdateParcer(appCommandRequest.Parameters);
                if (!result)
                {
                    Console.WriteLine("Invalid parameters");
                    return;
                }

                PropertyInfo[] updateProperties = GetProperties(properties, updateFieldsNames);
                PropertyInfo[] searchProperties = GetProperties(properties, searchFieldsNames);
                if (updateProperties is null || searchProperties is null)
                {
                    Console.WriteLine("No such properties found");
                    return;
                }

                if (updateProperties.Contains(typeof(FileCabinetRecord).GetProperty("Id")))
                {
                    Console.WriteLine("You can't change id");
                    return;
                }

                FileCabinetRecord updateRecord = GetRecord(updateProperties, updateFieldsNames, updateFieldsValues);
                FileCabinetRecord searchRecord = GetRecord(searchProperties, searchFieldsNames, searchFieldsValues);

                if (updateRecord is null || searchRecord is null)
                {
                    Console.WriteLine("Сould not match values to properties");
                    return;
                }

                try
                {
                    this.FileCabinetService.UpdateRecords(updateProperties, updateRecord, searchProperties, searchRecord);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
            else
            {
                if (this.NextCommandHandler != null)
                {
                    this.NextCommandHandler.Handle(appCommandRequest);
                }
                else
                {
                    throw new ArgumentException("Invalid command.");
                }
            }
        }

        private static FileCabinetRecord GetRecord(PropertyInfo[] properties, string[] fieldsNames, string[] fieldsValues)
        {
            var record = new FileCabinetRecord();
            foreach (var property in properties)
            {
                for (int i = 0; i < fieldsNames.Length; i++)
                {
                    if (string.Equals(fieldsNames[i], property.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            DateTime tempDate;
                            bool resultParce = DateTime.TryParseExact(fieldsValues[i], "M/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite, out tempDate);
                            if (resultParce)
                            {
                                record.DateOfBirth = tempDate;
                            }
                            else
                            {
                                Console.WriteLine("Incorrect date of birth");
                                return null;
                            }
                        }
                        else
                        {
                            try
                            {
                                property.SetValue(record, Convert.ChangeType(fieldsValues[i], property.PropertyType, CultureInfo.InvariantCulture), null);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine(ex.Message);
                                return null;
                            }
                        }

                        break;
                    }
                }
            }

            return record;
        }

        private static PropertyInfo[] GetProperties(PropertyInfo[] properties, string[] fieldsNames)
        {
            bool isVerified = false;
            var searchProperties = new List<PropertyInfo>();
            foreach (var name in fieldsNames)
            {
                isVerified = false;
                foreach (var property in properties)
                {
                    if (string.Equals(name, property.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isVerified = true;
                        searchProperties.Add(property);
                        break;
                    }
                }

                if (!isVerified)
                {
                    return null;
                }
            }

            return searchProperties.ToArray();
        }

        private static (string[] updateFieldsNames, string[] updateFieldsValues, string[] searchFieldsNames, string[] searchFieldsValues, bool result) UpdateParcer(string parameters)
        {
            int indexOfValue = parameters.IndexOf(" where ", 0, StringComparison.InvariantCultureIgnoreCase);
            if (indexOfValue == -1)
            {
                Console.WriteLine("Missing keyword 'where'");
                return (null, null, null, null, false);
            }

            string[] args = Regex.Split(parameters, " where ", RegexOptions.IgnoreCase);
            if (!args[0].StartsWith("set ", StringComparison.InvariantCultureIgnoreCase) || args.Length != 2)
            {
                return (null, null, null, null, true);
            }

            string[] updateParameters = Regex.Split(args[0][4..], "and", RegexOptions.IgnoreCase);
            string[] updateFieldsNames = new string[updateParameters.Length];
            string[] updateFieldsValues = new string[updateParameters.Length];
            for (int i = 0; i < updateParameters.Length; i++)
            {
                var temp = updateParameters[i].Replace(" ", string.Empty).Replace("'", string.Empty).Split("=");
                if (temp.Length != 2)
                {
                    Console.WriteLine("Insufficient number of parameters");
                    return (null, null, null, null, false);
                }
                else
                {
                    updateFieldsNames[i] = temp[0];
                    updateFieldsValues[i] = temp[1];
                }
            }

            string[] searchParameters = Regex.Split(args[1], "and", RegexOptions.IgnoreCase);
            string[] searchFieldsNames = new string[searchParameters.Length];
            string[] searchFieldsValues = new string[searchParameters.Length];
            for (int i = 0; i < searchParameters.Length; i++)
            {
                var temp = searchParameters[i].Replace(" ", string.Empty).Replace("'", string.Empty).Split("=");
                if (temp.Length != 2)
                {
                    Console.WriteLine("Insufficient number of parameters");
                    return (null, null, null, null, false);
                }
                else
                {
                    searchFieldsNames[i] = temp[0];
                    searchFieldsValues[i] = temp[1];
                }
            }

            return (updateFieldsNames, updateFieldsValues, searchFieldsNames, searchFieldsValues, true);
        }
    }
}
