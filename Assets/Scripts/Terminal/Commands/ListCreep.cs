using System.Collections.Generic;
using Gameplay.BoardPieces;
using UnityEngine;
using Utilities;

namespace Terminal.Commands
{
    public class ListCreep : ITerminalCommand
    {
        private const string Description = "Prints a list of creeps currently in play";
        private const string UsageSyntax = "list_creeps";

        private readonly List<string> commandAliases = new List<string>
        {
            "list_creep",
            "listcreep",
            "list_creeps",
            "listcreeps"
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
                    "Game is not in progress or is paused, cannot list creeps");
                return;
            }
            GameObject[] creepListSnapshot = GameUtils.GetWaveManager().creepList.ToArray();

            GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Header,
                StringUtils.FormatOutputString("Creep Type", "Creep ID", "Creep Health"));

            for (var i = 0; i < GameUtils.GetWaveManager().creepList.Count; i++)
            {
                string creepInfoString = StringUtils.FormatOutputString(creepListSnapshot[i].name, $"{i}",
                    $"{creepListSnapshot[i].GetComponent<BoardPieceLogic>().GetCurrentHealth()}/" +
                    $"{creepListSnapshot[i].GetComponent<BoardPieceLogic>().GetStartingHealth()}");

                GameUtils.GetTerminalLogic().WriteToTerminalOutput(TerminalOutputType.Info, creepInfoString);
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