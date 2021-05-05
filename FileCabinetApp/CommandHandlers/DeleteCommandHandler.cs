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
    /// This class provides a insert command hadler.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "delete", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] fieldsNames, fieldsValues;
                bool result;
                var properties = typeof(FileCabinetRecord).GetProperties();
                (fieldsNames, fieldsValues, result) = DeleteParcer(appCommandRequest.Parameters);
                if (!result)
                {
                    Console.WriteLine("Invalid parameters");
                    return;
                }

                properties = GetProperties(properties, fieldsNames);
                if (properties is null)
                {
                    Console.WriteLine("No such properties found");
                    return;
                }

                var record = GetRecord(properties, fieldsNames, fieldsValues);

                if (record is null)
                {
                    Console.WriteLine("Сould not match values to properties");
                    return;
                }

                Console.WriteLine(this.FileCabinetService.DeleteRecords(properties, record));
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

        private static (string[] fieldsNames, string[] fieldsValues, bool result) DeleteParcer(string parameters)
        {
            if (!parameters.StartsWith("where ", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Missing keyword 'where'");
                return (null, null, false);
            }

            string[] args = Regex.Split(parameters[6..], " and ", RegexOptions.IgnoreCase);
            string[] fieldsNames = new string[args.Length];
            string[] fieldsValues = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var temp = args[i].Replace(" ", string.Empty).Replace("'", string.Empty).Split("=");
                if (temp.Length != 2)
                {
                    Console.WriteLine("Insufficient number of parameters");
                    return (null, null, false);
                }
                else
                {
                    fieldsNames[i] = temp[0];
                    fieldsValues[i] = temp[1];
                }
            }

            if (fieldsValues.Length != fieldsNames.Length)
            {
                Console.WriteLine("The number of arguments passed does not match");
                return (null, null, false);
            }

            return (fieldsNames, fieldsValues, true);
        }
    }
}
