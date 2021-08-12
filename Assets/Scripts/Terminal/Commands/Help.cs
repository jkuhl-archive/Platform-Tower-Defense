using System.Collections.Generic;
using Utilities;

namespace Terminal.Commands
{
    public class Help : ITerminalCommand
    {
        private const string Description = "Displays all available commands";
        private const string UsageSyntax = "help";

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
                StringUtils.FormatOutputString("Command Name", "Usage Syntax", "Description"));

            foreach (var command in terminalLogic.GetAvailableCommands())
            {
                var info = StringUtils.FormatOutputString(command.GetCommandAliases()[0], command.GetUsageSyntax(),
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
        public string GetUsageSyntax()
        {
            return UsageSyntax;
        }
    }
}