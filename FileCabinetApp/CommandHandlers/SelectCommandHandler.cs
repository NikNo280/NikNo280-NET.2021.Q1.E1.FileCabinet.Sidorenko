using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Table;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a select command hadler.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandler
    {
        private Action<string> recordPrinter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        /// <param name="recordPrinter">Record printer.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<string> recordPrinter)
            : base(fileCabinetService)
        {
            this.recordPrinter = recordPrinter;
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

            if (string.Equals(appCommandRequest.Command, "select", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] printFieldsNames;
                List<List<(string, string)>> logicalСonditionsList;
                bool result;
                var propertiesFileCabinetRecord = typeof(FileCabinetRecord).GetProperties();
                (printFieldsNames, logicalСonditionsList, result) = SelectParcer(appCommandRequest.Parameters);

                if (!result)
                {
                    Console.WriteLine("Invalid parameters");
                    return;
                }

                var printProperties = GetProperties(propertiesFileCabinetRecord, printFieldsNames);
                if (printProperties is null)
                {
                    Console.WriteLine("No such properties found");
                    return;
                }

                var records = new FileCabinetRecord[logicalСonditionsList.Count];
                var searchProperties = new List<PropertyInfo[]>();

                for (int i = 0; i < logicalСonditionsList.Count; i++)
                {
                    var fieldsNames = new string[logicalСonditionsList[i].Count];
                    var fieldsValues = new string[logicalСonditionsList[i].Count];
                    for (int j = 0; j < logicalСonditionsList[i].Count; j++)
                    {
                        fieldsNames[j] = logicalСonditionsList[i][j].Item1;
                        fieldsValues[j] = logicalСonditionsList[i][j].Item2;
                    }

                    var searchProperty = GetProperties(propertiesFileCabinetRecord, fieldsNames);
                    if (searchProperty is null)
                    {
                        Console.WriteLine("No such properties found");
                        return;
                    }

                    searchProperties.Add(searchProperty);
                    var tempRecord = GetRecord(searchProperties[i], fieldsNames, fieldsValues);
                    if (tempRecord is null)
                    {
                        Console.WriteLine("Сould not match values to properties");
                        return;
                    }

                    records[i] = tempRecord;
                }

                var tableOfRecords = new TableOfRecords<FileCabinetRecord>(this.FileCabinetService.SelectRecords(searchProperties.ToArray(), records), printProperties);
                tableOfRecords.PrintTable(this.recordPrinter);
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

        private static PropertyInfo[] GetProperties(PropertyInfo[] properties, string[] fieldsNames)
        {
            bool isVerified;
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

        private static (string[] printProperties, List<List<(string, string)>> logicalСonditionsList, bool result) SelectParcer(string parameters)
        {
            string[] printFieldsName;
            int indexOfValue = parameters.IndexOf(" where ", 0, StringComparison.InvariantCultureIgnoreCase);
            if (indexOfValue == -1)
            {
                if (parameters.Equals("*"))
                {
                    var properties = typeof(FileCabinetRecord).GetProperties();
                    printFieldsName = new string[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        printFieldsName[i] = properties[i].Name;
                    }
                }
                else
                {
                    printFieldsName = parameters.Replace(" ", string.Empty).Split(',');
                }

                return (printFieldsName, new List<List<(string, string)>>(), true);
            }

            string[] args = Regex.Split(parameters, " where ", RegexOptions.IgnoreCase);
            if (args[0].Equals("*"))
            {
                var properties = typeof(FileCabinetRecord).GetProperties();
                printFieldsName = new string[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    printFieldsName[i] = properties[i].Name;
                }
            }
            else
            {
                printFieldsName = args[0].Replace(" ", string.Empty).Split(',');
            }

            var logicalСonditions = Regex.Split(args[1], " or ", RegexOptions.IgnoreCase);
            var logicalСonditionsList = new List<List<(string, string)>>();
            for (int i = 0; i < logicalСonditions.Length; i++)
            {
                var tempList = new List<(string, string)>();
                foreach (var item in Regex.Split(logicalСonditions[i], " and ", RegexOptions.IgnoreCase))
                {
                    var temp = item.Replace(" ", string.Empty).Replace("'", string.Empty).Split("=");
                    if (temp.Length != 2)
                    {
                        Console.WriteLine("Insufficient number of parameters");
                        return (null, null, false);
                    }
                    else
                    {
                        tempList.Add((temp[0], temp[1]));
                    }
                }

                logicalСonditionsList.Add(tempList);
            }

            return (printFieldsName, logicalСonditionsList, true);
        }
    }
}
