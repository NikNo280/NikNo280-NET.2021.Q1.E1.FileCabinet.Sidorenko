using FileCabinetGenerator.RandomRecord;
using System;
using System.Globalization;
using System.IO;

namespace FileCabinetGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var settings = LoadSettings(args);
            int startId, recordsAmount;
            bool result = int.TryParse(settings[2], out startId);
            if (!result)
            {
                throw new ArgumentException("startId is not a number");
            }

            result = int.TryParse(settings[3], out recordsAmount);
            if (!result)
            {
                throw new ArgumentException("recordsAmount is not a number");
            }

            var fileCabinet = new FileCabinet(new DefaultRandomRecord(), startId, recordsAmount);
            using (var streamW = new StreamWriter(settings[1]))
            {
                switch (settings[0].ToUpperInvariant())
                {
                    case "CSV":
                        fileCabinet.SaveToCsv(streamW);
                        break;
                    case "XML":
                        fileCabinet.SaveToXml(streamW);
                        break;
                }
            }
        }

        private static string[] LoadSettings(string[] args)
        {
            if (args.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid number of arguments");
            }

            string[] settings = new string[4];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains('='))
                {
                    string[] parameters = args[i].Split('=');
                    if (parameters.Length != 2 ||
                        string.IsNullOrWhiteSpace(parameters[0]) ||
                        string.IsNullOrWhiteSpace(parameters[1]))
                    {
                        throw new ArgumentException("Invalid arguments");
                    }
                    else
                    {
                        switch (args[i].Split('=')[0])
                        {
                            case "--output-type":
                                settings[0] = args[i].Split('=')[1];
                                break;
                            case "--output":
                                settings[1] = args[i].Split('=')[1];
                                break;
                            case "--records-amount":
                                settings[2] = args[i].Split('=')[1];
                                break;
                            case "--start-id":
                                settings[3] = args[i].Split('=')[1];
                                break;
                        }
                    }
                }
                else
                {
                    if (i + 1 >= args.Length)
                    {
                        throw new ArgumentException("Invalid arguments");
                    }

                    switch (args[i])
                    {
                        case "-t":
                            i++;
                            settings[0] = args[i];
                            break;
                        case "-o":
                            i++;
                            settings[1] = args[i];
                            break;
                        case "-a":
                            i++;
                            settings[2] = args[i];
                            break;
                        case "-i":
                            i++;
                            settings[3] = args[i];
                            break;
                    }
                }
            }

            foreach (var item in settings)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    throw new ArgumentException("Invalid arguments");
                }
            }

            return settings;
        }
    }
}
