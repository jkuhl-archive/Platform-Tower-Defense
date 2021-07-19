using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class MapLogic : MonoBehaviour
    {
        [Header("Platform and Platform Space Prefabs")]
        public GameObject platformPrefab;
        public GameObject platformSpacePrefab;
        
        [Header("Number of Platforms Allowed")]
        public int maxPlatformCount;
        
        [Header("Creep Navigation Nodes")]
        public List<GameObject> nodeList;
        
        [Header("Platform Spawn Points")]
        public List<Vector3> platformLocationList;
        
        [Header("Active Platform and Platform Spaces")]
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
}
