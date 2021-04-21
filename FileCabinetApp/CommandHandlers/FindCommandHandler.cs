using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a find command hadler.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "find", StringComparison.InvariantCultureIgnoreCase))
            {
                var parametersSplit = appCommandRequest.Parameters.Split(" ");
                if (parametersSplit.Length != 2 || string.IsNullOrWhiteSpace(parametersSplit[0]) || string.IsNullOrWhiteSpace(parametersSplit[1]))
                {
                    Console.WriteLine("Error input");
                    return;
                }

                ReadOnlyCollection<FileCabinetRecord> records = parametersSplit[0].ToUpperInvariant() switch
                {
                    "FIRSTNAME" => this.FileCabinetService.FindByFirstName(parametersSplit[1]),
                    "LASTNAME" => this.FileCabinetService.FindByLastName(parametersSplit[1]),
                    "DATEOFBIRTH" => this.FileCabinetService.FindByDateOfBirth(parametersSplit[1]),
                    _ => new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>())
                };
                if (records.Count == 0)
                {
                    Console.WriteLine("no records found");
                }
                else
                {
                    foreach (var item in records)
                    {
                        Console.WriteLine($"#{item.Id}, {item.FirstName}, {item.LastName}, " +
                            $"{item.Age}, {item.Salary}, {item.Gender}, " +
                            $"{item.DateOfBirth.ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture)}");
                    }
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
    }
}
