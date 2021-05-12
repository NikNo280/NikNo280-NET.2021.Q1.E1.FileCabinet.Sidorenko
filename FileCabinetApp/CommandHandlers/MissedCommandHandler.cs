using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a handler if no suitable handler exists.
    /// </summary>
    public class MissedCommandHandler : CommandHandlerBase
    {
        private static string[] commandsNames = new string[]
        {
            "help", "exit", "insert", "delete", "update", "stat", "create", "export", "import", "purge", "select",
        };

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

            if (string.IsNullOrWhiteSpace(appCommandRequest.Command))
            {
                Console.WriteLine("See 'help'");
                return;
            }

            var helpList = new List<string>();
            foreach (var commandName in commandsNames)
            {
                if (commandName.Contains(appCommandRequest.Command, StringComparison.InvariantCultureIgnoreCase) ||
                    (commandName.StartsWith(appCommandRequest.Command[0]) && commandName.EndsWith(appCommandRequest.Command[^1])))
                {
                    helpList.Add(commandName);
                }
            }

            Console.WriteLine($"There is no '{appCommandRequest.Command}' command. See 'help'");
            if (helpList.Count == 1)
            {
                Console.WriteLine("The most similar command is");
            }
            else if (helpList.Count > 1)
            {
                Console.WriteLine("The most similar commands are");
            }

            foreach (var commandName in helpList)
            {
                Console.WriteLine($"\t{commandName}");
            }

            Console.WriteLine();
        }
    }
}
