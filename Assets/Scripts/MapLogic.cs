using System;
using System.Collections.Generic;
using UnityEngine;

public class MapLogic : MonoBehaviour
{
    // Platform prefab GameObjects
    public GameObject platformSpacePrefab;
    public GameObject platformPrefab;
    
    // Map interaction variables
    public List<GameObject> nodeList;
    public List<Vector3> platformLocationList;
    public List<GameObject> platformSpaceList;
    public List<GameObject> platformList;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializtionChecks();

        foreach (Vector3 spawnLocation in platformLocationList)
        {
            GameObject newPlatformSpace = Instantiate(platformSpacePrefab, spawnLocation, platformSpacePrefab.transform.rotation);
            platformSpaceList.Add(newPlatformSpace);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Verifies all required variables are valid
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when a variable is null or otherwise invalid</exception>
    void InitializtionChecks()
    {
        if (platformSpacePrefab == null)
        {
            throw new ArgumentNullException("platformSpacePrefab is null");
        }
        
        if (platformPrefab == null)
        {
            throw new ArgumentNullException("platformPrefab is null");
        }
        
        if (nodeList == null)
        {
            throw new ArgumentNullException("nodeList is null");
        }

        if (nodeList.Count < 2)
        {
            throw new ArgumentNullException("nodeList should have at least 2 nodes");
        }
        
        if (platformLocationList == null)
        {
            throw new ArgumentNullException("platformLocationList is null");
        }

        if (platformLocationList.Count < 2)
        {
            throw new ArgumentNullException("platformLocationList should have at least 2 items");
        }
    }
}
