using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Terminal.Commands
{
    public class ListTower : ITerminalCommand
    {
        private const string Description = "Prints a list of towers currently in play";
        private const string UsageSyntax = "list_tower";

        private readonly List<string> commandAliases = new List<string>
        {
            "list_tower",
            "listtower",
            "list_towers",
            "listtowers"
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
                    "Game is not in progress or is paused, cannot list towers");
                return;
            }
            
            Dictionary<Tuple<int, int>, GameObject> activeTowers = GameUtils.GetMapLogic().GetActiveTowers();

            GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Header,
                StringUtils.FormatOutputString("Tower Type", "Platform Number", "Socket Number"));

            foreach (KeyValuePair<Tuple<int, int>, GameObject> tower in activeTowers)
            {
                string towerInfoString = StringUtils.FormatOutputString(tower.Value.name,
                    tower.Key.Item1.ToString(), tower.Key.Item2.ToString());
                
                GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Info, towerInfoString);
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