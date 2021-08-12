using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Terminal.Commands
{
    public class GiveBuff : ITerminalCommand
    {
        private const string Description = "Gives a buff to a creep or tower";
        private const string UsageSyntax = "give_buff tower 0,0 bleed";

        private readonly List<string> commandAliases = new List<string>
        {
            "give_buff",
            "givebuff"
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
                    "Game is not in progress or is paused, cannot give buff");
                return;
            }

            if (args.Count != 3)
            {
                GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                    "Invalid number of arguments, run 'help' for more details");
                return;
            }

            // Determine target
            GameObject target = null;
            switch (args[0])
            {
                case "tower":
                    string[] towerID = args[1].Split(',');
                    if (!int.TryParse(towerID[0], out var platformIndex) ||
                        !int.TryParse(towerID[1], out var socketIndex))
                    {
                        GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                            $"Could not parse tower ID from '{args[1]}', Usage: '{GetUsageSyntax()}'");
                        return;
                    }

                    target = GameUtils.GetMapLogic().GetTowerByIndex(platformIndex, socketIndex);
                    break;

                case "creep":
                    if (!int.TryParse(args[1], out var creepIndex))
                    {
                        GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                            $"Could not parse creep ID from '{args[1]}', Usage: '{GetUsageSyntax()}'");
                    }

                    if (GameUtils.GetWaveManager().creepList.Count > creepIndex)
                    {
                        target = GameUtils.GetWaveManager().creepList[creepIndex];
                    }
                    else
                    {
                        GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                            $"Creep ID '{creepIndex}' does not exist");
                    }
                    break;
            }

            if (target == null)
            {
                GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Error,
                    "Invalid target, cannot give buff");
                return;
            }
            
            GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Info,
                $"Applying buff '{args[2]}' to {args[0]} '{target.name}'");
            GameUtils.GetBuffController().ApplyBuffByName(target, args[2]);
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