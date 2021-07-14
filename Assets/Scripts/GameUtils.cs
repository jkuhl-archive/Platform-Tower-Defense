using System;
using UnityEngine;

public class GameUtils
{
    /// <summary>
    /// Gets a named GameObject from the root of the scene and returns it
    /// </summary>
    /// <returns>GameObject with the given name</returns>
    /// <exception cref="NullReferenceException">If unable to locate GameObject</exception>
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

        if (namedGameObject == null)
        {
            throw new NullReferenceException($"Unable to access {gameObjectName} GameObject");
        }

        return namedGameObject;
    }
}
