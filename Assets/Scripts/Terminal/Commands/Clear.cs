using System.Collections.Generic;
using Utilities;

namespace Terminal.Commands
{
    public class Clear : ITerminalCommand
    {
        private const string Description = "Gives the player the given amount of health";
        private const string UsageInfo = "clear";

        private readonly List<string> commandAliases = new List<string>
        {
            "clear",
            "cls"
        };

        /// <summary>
        ///     Logic to be preformed when the command is executed
        /// </summary>
        /// <param name="args"> Arguments that were passed in with the command </param>
        public void Execute(List<string> args)
        {
            GameUtils.GetTerminalLogic().TrimOutputLines(0);
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
    }
}