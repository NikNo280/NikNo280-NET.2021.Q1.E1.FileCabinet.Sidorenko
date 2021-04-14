using FileCabinetGenerator.RandomRecord;
using System;
using System.Globalization;

namespace FileCabinetGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            DefaultRandomRecord defaultRandomRecord = new DefaultRandomRecord();

            foreach (var item in LoadSettings(args))
            {
                defaultRandomRecord.GetRandomName();
                Console.WriteLine(defaultRandomRecord.GetRandomName());
                Console.WriteLine(defaultRandomRecord.GetRandomDateOfBirth().ToString("yyyy-MMM-dd", CultureInfo.InvariantCulture));
                Console.WriteLine(defaultRandomRecord.GetRandomAge());
                Console.WriteLine(defaultRandomRecord.GetRandomGender());
                Console.WriteLine(defaultRandomRecord.GetRandomSalary());
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
