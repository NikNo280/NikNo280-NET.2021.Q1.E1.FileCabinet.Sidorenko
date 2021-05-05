using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using FileCabinetApp.Validation.InputValidation;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a insert command hadler.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "insert", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] fieldsNames, fieldsValues;
                bool result;
                (fieldsNames, fieldsValues, result) = InsertParcer(appCommandRequest.Parameters);
                if (!result)
                {
                    Console.WriteLine("Invalid parameters");
                    return;
                }

                bool createId = true;
                var record = new FileCabinetRecord();

                foreach (var property in typeof(FileCabinetRecord).GetProperties())
                {
                    if (property.Name.ToUpperInvariant() == "ID")
                    {
                        for (int i = 0; i < fieldsNames.Length; i++)
                        {
                            if (string.Equals(fieldsNames[i], property.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                createId = false;
                                try
                                {
                                    property.SetValue(record, Convert.ChangeType(fieldsValues[i], property.PropertyType, CultureInfo.InvariantCulture), null);
                                }
                                catch (FormatException ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    return;
                                }
                            }
                        }

                        continue;
                    }

                    if (!FieldsConverter(property, fieldsNames, fieldsValues, record))
                    {
                        Console.WriteLine("Insufficient number of parameters, only id can be skipped");
                        return;
                    }
                }

                if (createId)
                {
                    record.Id = this.FileCabinetService.GetLastIndex() + 1;
                }

                try
                {
                    this.FileCabinetService.InsertRecord(record);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
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

        private static bool FieldsConverter(PropertyInfo property, string[] fieldsNames, string[] fieldsValues, FileCabinetRecord record)
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
                            return false;
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
                            return false;
                        }
                    }

                    break;
                }
            }

            return true;
        }

        private static (string[] fieldsNames, string[] fieldsValues, bool result) InsertParcer(string parameters)
        {
            int indexOfValue = parameters.IndexOf("values", 0, StringComparison.InvariantCultureIgnoreCase);
            if (indexOfValue == -1)
            {
                Console.WriteLine("Missing keyword 'values'");
                return (null, null, false);
            }

            string[] fieldsNames = null, fieldsValues = null;
            fieldsNames = parameters[..indexOfValue].Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Split(',');
            fieldsValues = parameters[(indexOfValue + 6) ..].Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("'", string.Empty).Split(',');

            if (fieldsValues.Length != fieldsNames.Length)
            {
                Console.WriteLine("The number of arguments passed does not match");
                return (null, null, false);
            }

            return (fieldsNames, fieldsValues, true);
        }
    }
}
