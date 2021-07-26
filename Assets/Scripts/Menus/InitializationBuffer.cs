using System.IO;
using UnityEngine;
using Utilities;

namespace Menus
{
    public static class InitializationBuffer
    {
        // Prefab directory variables
        private static readonly string PrefabsDirectory = "Prefabs";
        private static readonly string GameModesDirectory = Path.Combine(PrefabsDirectory, "GameModes");
        private static readonly string MapsDirectory = Path.Combine(PrefabsDirectory, "Maps");

        // Game mode specific prefabs
        private static GameObject _gameLogic;
        private static GameObject _gameUI;

        // Map specific prefabs
        private static GameObject _backgroundMusic;
        private static GameObject _lighting;
        private static GameObject _mainCamera;
        private static GameObject _map;

        /// <summary>
        ///     Resets all values in the buffer back to null
        /// </summary>
        public static void ResetBuffer()
        {
            _gameLogic = null;
            _gameUI = null;
            _backgroundMusic = null;
            _lighting = null;
            _mainCamera = null;
            _map = null;
        }

        /// <summary>
        ///     Checks if all required values in the buffer have been populated
        /// </summary>
        /// <returns> True if all of the values in the buffer have been set, false if not </returns>
        public static bool BufferReady()
        {
            if (_gameLogic == null)
            {
                Debug.Log("InitializationBuffer not ready, gameLogic is null");
                return false;
            }

            if (_gameUI == null)
            {
                Debug.Log("InitializationBuffer not ready, gameUI is null");
                return false;
            }

            if (_backgroundMusic == null)
            {
                Debug.Log("InitializationBuffer not ready, backgroundMusic is null");
                return false;
            }

            if (_lighting == null)
            {
                Debug.Log("InitializationBuffer not ready, lighting is null");
                return false;
            }

            if (_mainCamera == null)
            {
                Debug.Log("InitializationBuffer not ready, mainCamera is null");
                return false;
            }

            if (_map == null)
            {
                Debug.Log("InitializationBuffer not ready, map is null");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Gets the background music for the scene
        /// </summary>
        /// <returns> Background music prefab </returns>
        public static GameObject GetBackgroundMusic()
        {
            return _backgroundMusic;
        }

        /// <summary>
        ///     Gets the game logic for the scene
        /// </summary>
        /// <returns> Game logic prefab </returns>
        public static GameObject GetGameLogic()
        {
            return _gameLogic;
        }

        /// <summary>
        ///     Gets the game UI for the scene
        /// </summary>
        /// <returns> Game UI prefab </returns>
        public static GameObject GetGameUI()
        {
            return _gameUI;
        }

        /// <summary>
        ///     Gets the lighting for the scene
        /// </summary>
        /// <returns> Lighting prefab </returns>
        public static GameObject GetLighting()
        {
            return _lighting;
        }

        /// <summary>
        ///     Gets the main camera for the scene
        /// </summary>
        /// <returns> Main camera prefab </returns>
        public static GameObject GetMainCamera()
        {
            return _mainCamera;
        }

        /// <summary>
        ///     Gets the map for the scene
        /// </summary>
        /// <returns> Game map prefab </returns>
        public static GameObject GetMap()
        {
            return _map;
        }

        /// <summary>
        ///     Stores the selected maps prefabs in the buffer
        /// </summary>
        /// <param name="mapName"> Name of the game map </param>
        /// <returns> True if able to store selected maps's prefabs, false if not </returns>
        public static bool SelectGameMap(string mapName)
        {
            var mapDirectory = Path.Combine(MapsDirectory, mapName);
            var backgroundMusicPrefabPath = Path.Combine(mapDirectory, "BackgroundMusic");
            var lightingPrefabPath = Path.Combine(mapDirectory, "Lighting");
            var mainCameraPrefabPath = Path.Combine(mapDirectory, "MainCamera");
            var mapPrefabPath = Path.Combine(mapDirectory, "Map");

            Debug.Log($"Selected game map: '{mapName}'");

            if (GameUtils.ResourceDirectoryExists(mapDirectory))
            {
                // Load background music prefab
                if (GameUtils.ResourceFileExists(backgroundMusicPrefabPath, "prefab"))
                {
                    var backgroundMusicPrefab = Resources.Load<GameObject>(backgroundMusicPrefabPath);
                    var success = SetBackgroundMusic(backgroundMusicPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load background music prefab: '{backgroundMusicPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log(
                        $"Map directory does not contain a background music prefab: '{backgroundMusicPrefabPath}'");
                    return false;
                }

                // Load lighting prefab
                if (GameUtils.ResourceFileExists(lightingPrefabPath, "prefab"))
                {
                    var lightingPrefab = Resources.Load<GameObject>(lightingPrefabPath);
                    var success = SetLighting(lightingPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load lighting prefab: '{lightingPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Map directory does not contain a lighting prefab: '{lightingPrefabPath}'");
                    return false;
                }

                // Load main camera prefab
                if (GameUtils.ResourceFileExists(mainCameraPrefabPath, "prefab"))
                {
                    var mainCameraPrefab = Resources.Load<GameObject>(mainCameraPrefabPath);
                    var success = SetMainCamera(mainCameraPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load main camera prefab: '{mainCameraPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Map directory does not contain a main camera prefab: '{mainCameraPrefabPath}'");
                    return false;
                }

                // Load map prefab
                if (GameUtils.ResourceFileExists(mapPrefabPath, "prefab"))
                {
                    var mapPrefab = Resources.Load<GameObject>(mapPrefabPath);
                    var success = SetMap(mapPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load map prefab: '{mapPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Map directory does not contain a map prefab: '{mapPrefabPath}'");
                    return false;
                }
            }
            else
            {
                Debug.Log($"Map directory does not exist: '{mapDirectory}'");
                return false;
            }

            return true;
        }
        
        /// <summary>
        ///     Stores the selected game mode's prefabs in the buffer
        /// </summary>
        /// <param name="gameModeName"> Name of the game mode </param>
        /// <returns> True if able to store selected game mode's prefabs, false if not </returns>
        public static bool SelectGameMode(string gameModeName)
        {
            var gameModeDirectory = Path.Combine(GameModesDirectory, gameModeName);
            var gameLogicPrefabPath = Path.Combine(gameModeDirectory, "GameLogic");
            var gameUiPrefabPath = Path.Combine(gameModeDirectory, "GameUI");

            Debug.Log($"Selected game mode: '{gameModeName}'");

            if (GameUtils.ResourceDirectoryExists(gameModeDirectory))
            {
                // Load game logic prefab
                if (GameUtils.ResourceFileExists(gameLogicPrefabPath, "prefab"))
                {
                    var gameLogicPrefab = Resources.Load(Path.Combine(gameModeDirectory, "GameLogic")) as GameObject;
                    var success = SetGameLogic(gameLogicPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load game logic prefab: '{gameLogicPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Game logic prefab does not exist: '{gameLogicPrefabPath}'");
                    return false;
                }

                // Load game UI prefab
                if (GameUtils.ResourceFileExists(gameUiPrefabPath, "prefab"))
                {
                    var gameUiPrefab = Resources.Load<GameObject>(gameUiPrefabPath);
                    var success = SetGameUI(gameUiPrefab);

                    if (!success)
                    {
                        Debug.Log($"Failed to load game ui prefab: '{gameUiPrefabPath}'");
                        return false;
                    }
                }
                else
                {
                    Debug.Log($"Game ui prefab does not exist: '{gameUiPrefabPath}'");
                    return false;
                }
            }
            else
            {
                Debug.Log($"Game mode directory does not exist: '{gameModeDirectory}'");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Sets the background music for the scene if there is not a background music defined already
        /// </summary>
        /// <param name="backgroundMusic"> Prefab for the background music that we want to use in the scene </param>
        /// <returns> True if able to successfully set background music, false if not </returns>
        public static bool SetBackgroundMusic(GameObject backgroundMusic)
        {
            if (_backgroundMusic == null)
            {
                _backgroundMusic = backgroundMusic;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the game logic for the scene if there is not a game logic defined already
        /// </summary>
        /// <param name="gameLogic"> Prefab for the game logic that we want to use in the scene </param>
        /// <returns> True if able to successfully set game logic, false if not </returns>
        public static bool SetGameLogic(GameObject gameLogic)
        {
            if (_gameLogic == null && gameLogic != null)
            {
                _gameLogic = gameLogic;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the game UI for the scene if there is not a game UI defined already
        /// </summary>
        /// <param name="gameUI"> Prefab for the game UI that we want to use in the scene </param>
        /// <returns> True if able to successfully set game UI, false if not </returns>
        public static bool SetGameUI(GameObject gameUI)
        {
            if (_gameUI == null)
            {
                _gameUI = gameUI;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the lighting for the scene if there is not a lighting defined already
        /// </summary>
        /// <param name="lighting"> Prefab for the lighting that we want to use in the scene </param>
        /// <returns> True if able to successfully set lighting, false if not </returns>
        public static bool SetLighting(GameObject lighting)
        {
            if (_lighting == null)
            {
                _lighting = lighting;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the main camera for the scene if there is not a main camera defined already
        /// </summary>
        /// <param name="mainCamera"> Prefab for the main camera that we want to use in the scene </param>
        /// <returns> True if able to successfully set main camera, false if not </returns>
        public static bool SetMainCamera(GameObject mainCamera)
        {
            if (_mainCamera == null)
            {
                _mainCamera = mainCamera;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the map for the scene if there is not a game map defined already
        /// </summary>
        /// <param name="map"> Prefab for the game map that we want to use in the scene </param>
        /// <returns> True if able to successfully set map, false if not </returns>
        public static bool SetMap(GameObject map)
        {
            if (_map == null)
            {
                _map = map;
                return true;
            }

            return false;
        }
    }
}