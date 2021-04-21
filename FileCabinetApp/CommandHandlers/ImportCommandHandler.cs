using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a import command hadler.
    /// </summary>
    public class ImportCommandHandler : CommandHandlerBase
    {
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

            if (string.Equals(appCommandRequest.Command, "import", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] data = appCommandRequest.Parameters.Split(' ');
                if (data.Length != 2)
                {
                    Console.WriteLine("Incorrect parameters");
                    return;
                }

                try
                {
                    using (var streamReader = new StreamReader(data[1]))
                    {
                        var snapshot = Program.fileCabinetService.MakeSnapshot();
                        switch (data[0].ToUpperInvariant())
                        {
                            case "CSV":
                                snapshot.LoadFromCsv(streamReader);
                                Program.fileCabinetService.Restore(snapshot);
                                Console.WriteLine($"{snapshot.RecordsImportCount} records were imported from {data[1]}");
                                break;
                            case "XML":
                                snapshot.LoadFromXml(streamReader);
                                Program.fileCabinetService.Restore(snapshot);
                                Console.WriteLine($"{snapshot.RecordsImportCount} records were imported from {data[1]}");
                                break;
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine($"Import error: file {data[1]} is not exist.");
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
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
