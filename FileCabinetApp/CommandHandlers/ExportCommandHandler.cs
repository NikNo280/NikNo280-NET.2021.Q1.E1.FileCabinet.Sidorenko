using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a export command hadler.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (string.Equals(appCommandRequest.Command, "export", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] data = appCommandRequest.Parameters.Split(' ');
                if (data.Length != 2)
                {
                    Console.WriteLine("Incorrect parameters");
                    return;
                }

                try
                {
                    var snapshot = this.FileCabinetService.MakeSnapshot();
                    if (File.Exists(data[1]))
                    {
                        Console.Write($"File is exist - rewrite {data[1]}? [Y/n] ");
                        string rewriting = Console.ReadLine();
                        if (rewriting.ToUpperInvariant() != "Y")
                        {
                            return;
                        }
                    }

                    using (var streamW = new StreamWriter(data[1]))
                    {
                        switch (data[0].ToUpperInvariant())
                        {
                            case "CSV":
                                snapshot.SaveToCsv(streamW);
                                Console.WriteLine($"All records are exported to file {data[1]}.");
                                break;
                            case "XML":
                                snapshot.SaveToXml(streamW);
                                Console.WriteLine($"All records are exported to file {data[1]}.");
                                break;
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
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
