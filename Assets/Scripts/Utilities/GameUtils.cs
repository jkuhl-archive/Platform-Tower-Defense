using System.IO;
using Gameplay;
using Gameplay.BoardPieces;
using Gameplay.Buffs;
using Gameplay.WaveLogic;
using Menus;
using Terminal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class GameUtils
    {
        // Active gameplay status variables
        private static bool _gameStarted;
        private static bool _gameInProgress;
        private static bool _gamePaused;

        // Static location for storing a reference to objects that are never destroyed
        private static GameObject _gameLoadingObject;
        private static GameObject _terminalObject;

        /// <summary>
        ///     Exits the application
        /// </summary>
        public static void ExitGame()
        {
            if (Application.isEditor) return;

            Application.Quit(0);
        }

        /// <summary>
        ///     Triggers the game over logic
        /// </summary>
        public static void Gameplay_GameOver()
        {
            Debug.Log("Game Over!");
            _gameInProgress = false;
        }

        /// <summary>
        ///     Handles pausing and un-pausing gameplay
        /// </summary>
        /// <param name="enabled"> True to pause the game, false to un-pause </param>
        public static void Gameplay_PauseGame(bool enabled)
        {
            _gamePaused = enabled;
        }

        /// <summary>
        ///     Resets the game state variables back to stock
        /// </summary>
        public static void Gameplay_ResetGame()
        {
            _gameStarted = false;
            _gameInProgress = false;
            _gamePaused = false;
        }

        /// <summary>
        ///     Starts the game
        /// </summary>
        public static void Gameplay_StartGame()
        {
            if (!_gameStarted)
            {
                Debug.Log("Starting game...");
                if (GetWaveManager().StartSpawning())
                {
                    _gameStarted = true;
                    _gameInProgress = true;
                    _gamePaused = false;
                }
            }
        }

        /// <summary>
        ///     Gets a named GameObject from the root of the scene and returns it
        /// </summary>
        /// <returns> GameObject with the given name </returns>
        public static GameObject GetRootGameObjectByName(string gameObjectName)
        {
            GameObject namedGameObject = null;

            var sceneObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var obj in sceneObjects)
                if (obj.name.Equals(gameObjectName))
                    namedGameObject = obj;

            return namedGameObject;
        }

        /// <summary>
        ///     Gets the BaseGameUI object from the current scene
        /// </summary>
        /// <returns> BaseGameUI object or null if it could not be found </returns>
        public static BaseGameUI GetBaseGameUI()
        {
            return GetRootGameObjectByName("GameUI").GetComponent<BaseGameUI>();
        }

        public static BuffController GetBuffController()
        {
            return GetRootGameObjectByName("GameLogic").GetComponent<BuffController>();
        }

        /// <summary>
        ///     Gets the GameLoadingLogic object
        /// </summary>
        /// <returns> GameLoadingLogic object or null if it could not be found </returns>
        public static GameLoadingLogic GetGameLoadingLogic()
        {
            return _gameLoadingObject.GetComponent<GameLoadingLogic>();
        }

        /// <summary>
        ///     Gets the MapLogic object from the current scene
        /// </summary>
        /// <returns> MapLogic object or null if it could not be found </returns>
        public static MapLogic GetMapLogic()
        {
            return GetRootGameObjectByName("Map").GetComponent<MapLogic>();
        }

        /// <summary>
        ///     Gets the PlayerLogic object from the current scene
        /// </summary>
        /// <returns> PlayerLogic object or null if it could not be found </returns>
        public static PlayerLogic GetPlayerLogic()
        {
            return GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>();
        }
        
        /// <summary>
        ///     Gets the TerminalLogic object
        /// </summary>
        /// <returns> TerminalLogic object or null if it could not be found </returns>
        public static TerminalLogic GetTerminalLogic()
        {
            return _terminalObject.GetComponent<TerminalLogic>();
        }

        /// <summary>
        ///     Gets the WaveManager object from the current scene
        /// </summary>
        /// <returns> WaveManager object or null if it could not be found </returns>
        public static WaveManager GetWaveManager()
        {
            return GetRootGameObjectByName("GameLogic").GetComponent<WaveManager>();
        }

        /// <summary>
        ///     Checks if gameplay is in progress
        /// </summary>
        /// <returns> True if gameplay is in progress, false if not </returns>
        public static bool IsGameInProgress()
        {
            return _gameInProgress;
        }

        /// <summary>
        ///     Checks if the game is paused
        /// </summary>
        /// <returns> True if the game is paused, false if not </returns>
        public static bool IsGamePaused()
        {
            return _gamePaused;
        }

        /// <summary>
        ///     Checks if the given directory exists in the game's 'Resources' directory
        /// </summary>
        /// <param name="resourceDirectoryPath"> Relative path to the directory in the 'Resources' directory </param>
        /// <returns> True if the directory exists, false if not </returns>
        public static bool ResourceDirectoryExists(string resourceDirectoryPath)
        {
            if (Application.isEditor)
            {
                var directoryPath = Path.Combine(Application.dataPath, "Resources", resourceDirectoryPath);
                return Directory.Exists(directoryPath);
            }
            
            // We always return true if not in the editor as we assume the folder exists in production
            return true;
        }

        /// <summary>
        ///     Checks if the given file exists in the game's 'Resources' directory
        ///     Note that the file extension should NOT be included in the file path and is passed in as a separate argument
        /// </summary>
        /// <param name="resourceFilePath"> Relative path to the file in the 'Resources' directory </param>
        /// <param name="fileExtension"> File extension of the file in question </param>
        /// <returns> True if the file exists, false if not </returns>
        public static bool ResourceFileExists(string resourceFilePath, string fileExtension)
        {
            if (Application.isEditor)
            {
                var filePath = Path.Combine(Application.dataPath, "Resources", $"{resourceFilePath}.{fileExtension}");
                return File.Exists(filePath);
            }

            // We always return true if not in the editor as we assume the file exists in production
            return true;
        }

        /// <summary>
        ///     Stores a static reference to the GameLoadingObject
        /// </summary>
        /// <param name="gameLoadingObject"> GameLoadingObject GameObject </param>
        /// <returns> True if able to store, false if not. Returns false if a GameLoadingObject has already been stored. </returns>
        public static bool SetGameLoadingObject(GameObject gameLoadingObject)
        {
            if (_gameLoadingObject == null)
            {
                _gameLoadingObject = gameLoadingObject;
                return true;
            }

            return false;
        }
        
        /// <summary>
        ///     Stores a static reference to the TerminalObject
        /// </summary>
        /// <param name="terminalObject"> TerminalObject GameObject </param>
        /// <returns> True if able to store, false if not. Returns false if a TerminalObject has already been stored. </returns>
        public static bool SetTerminalObject(GameObject terminalObject)
        {
            if (_terminalObject == null)
            {
                _terminalObject = terminalObject;
                return true;
            }

            return false;
        }
    }
}