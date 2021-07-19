using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public static class GameUtils
    {
        // Game scene names
        public static string scene_mainMenu = "MainMenu";
        public static string scene_classicTD = "ClassicTD";

        private static GameObject selectedMapPrefab;
    
        /// <summary>
        /// Gets a named GameObject from the root of the scene and returns it
        /// </summary>
        /// <returns>GameObject with the given name</returns>
        public static GameObject GetRootGameObjectByName(string gameObjectName)
        {
            GameObject namedGameObject = null;
        
            GameObject[] sceneObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject obj in sceneObjects)
            {
                if (obj.name.Equals(gameObjectName))
                {
                    namedGameObject = obj;
                }
            }

            return namedGameObject;
        }

        /// <summary>
        /// Stores the selected game map prefab and then attempts to load the given game mode scene
        /// </summary>
        /// <param name="gameModeSceneName"> Name of the game mode Unity scene that we want to load </param>
        /// <param name="mapPrefab"> Game map prefab GameObject that we want to load once the game scene is loaded </param>
        public static void StartNewGame(string gameModeSceneName, GameObject mapPrefab)
        {
            if (gameModeSceneName != null && mapPrefab != null)
            {
                selectedMapPrefab = mapPrefab;
                SceneManager.LoadScene(gameModeSceneName);
            }
        }

        /// <summary>
        /// Gets the selected game map prefab
        /// </summary>
        /// <returns> Selected game map prefab GameObject </returns>
        public static GameObject GetSelectedMap()
        {
            GameObject temp = selectedMapPrefab;
            selectedMapPrefab = null;
            return temp;
        }
    }
}
