using System.Collections.Generic;
using Utilities;

namespace Terminal.Commands
{
    public class Help : ITerminalCommand
    {
        private const string Description = "Displays all available commands and their help information";
        private const string UsageInfo = "help";

        private readonly List<string> commandAliases = new List<string>
        {
            "help",
            "h"
        };

        /// <summary>
        ///     Logic to be preformed when the command is executed
        /// </summary>
        /// <param name="args"> Arguments that were passed in with the command </param>
        public void Execute(List<string> args)
        {
            var terminalLogic = GameUtils.GetTerminalLogic();

            terminalLogic.WriteToTerminalOutput(TerminalOutputType.Header,
                GetFormattedString("Command Name", "Usage Info", "Description"));

            foreach (var command in terminalLogic.GetAvailableCommands())
            {
                var info = GetFormattedString(command.GetCommandAliases()[0], command.GetUsageInfo(),
                    command.GetDescription());
                terminalLogic.WriteToTerminalOutput(TerminalOutputType.Info, info);
            }
        }

        /// <summary>
        ///     Gets the list of possible aliases for the command
        /// </summary>
        /// <returns> List of strings containing aliases for the command </returns>
        public List<string> GetCommandAliases()
        {
            return commandAliases;
        }

        /// <summary>
        ///     Gets the description of the command
        /// </summary>
        /// <returns> String containing description of the command </returns>
        public string GetDescription()
        {
            return Description;
        }

        /// <summary>
        ///     Gets the usage syntax info for the command
        /// </summary>
        /// <returns> String containing usage syntax information for the command </returns>
        public string GetUsageInfo()
        {
            return UsageInfo;
        }

        /// <summary>
        ///     Generates a tab formatted string to make the help output look pretty
        /// </summary>
        /// <param name="string1"> First string to be included in the formatted string </param>
        /// <param name="string2"> Second string to be included in the formatted string </param>
        /// <param name="string3"> Third string to be included in the formatted string </param>
        /// <returns> Formatted string containing the 3 input strings </returns>
        private string GetFormattedString(string string1, string string2, string string3)
        {
            // Pads a given string with spaces to the desired segment length
            string AutoFormat(string baseString, int segmentLength)
            {
                var remainder = segmentLength - baseString.Length;
                return $"{baseString}{new string(' ', remainder)}";
            }

            return $"{AutoFormat(string1, 20)}" +
                   $"{AutoFormat(string2, 20)}" +
                   $"{AutoFormat(string3, 60)}";
        }
    }
}