using System.Collections.Generic;

namespace Terminal
{
    public interface ITerminalCommand
    {
        /// <summary>
        ///     Logic to be preformed when the command is executed
        /// </summary>
        /// <param name="args"> Arguments that were passed in with the command </param>
        void Execute(List<string> args);

        /// <summary>
        ///     Gets the list of possible aliases for the command
        ///     Example: ["givemoney", "givegold", "giveincome"]
        /// </summary>
        /// <returns> List of strings containing aliases for the command </returns>
        List<string> GetCommandAliases();

        /// <summary>
        ///     Gets the description of the command
        ///     Example: "Adds the given amount of money to the player's current money"
        /// </summary>
        /// <returns> String containing description of the command </returns>
        string GetDescription();

        /// <summary>
        ///     Gets the usage syntax info for the command
        ///     Example "givemoney 100"
        /// </summary>
        /// <returns> String containing usage syntax information for the command </returns>
        string GetUsageInfo();
    }
}