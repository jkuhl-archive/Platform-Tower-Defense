using System.Collections.Generic;
using Utilities;

namespace Terminal.Commands
{
    public class GiveHealth : ITerminalCommand
    {
        private const string Description = "Gives the player the given amount of health";
        private const string UsageSyntax = "give_health 100";

        private readonly List<string> commandAliases = new List<string>
        {
            "give_health",
            "givehealth"
        };

        /// <summary>
        ///     Logic to be preformed when the command is executed
        /// </summary>
        /// <param name="args"> Arguments that were passed in with the command </param>
        public void Execute(List<string> args)
        {
            if (!GameUtils.IsGameInProgress() || GameUtils.IsGamePaused())
            {
                GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                    "Game is not in progress or is paused, cannot give player health");
                return;
            }

            var amount = 0;
            if (args.Count > 0)
            {
                var success = int.TryParse(args[0], out amount);
                if (!success)
                {
                    GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                        $"'{args[0]}' could not be parsed to an int, Usage: '{GetUsageSyntax()}'");
                    return;
                }
            }

            GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Info,
                $"Giving player {amount} HP");
            GameUtils.GetPlayerLogic().UpdatePlayerHealth(amount);
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