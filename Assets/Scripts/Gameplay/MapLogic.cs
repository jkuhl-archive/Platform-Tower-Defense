using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class MapLogic : MonoBehaviour
    {
        [Header("Platform and Platform Space Prefabs")]
        [SerializeField] private GameObject platformPrefab;
        [SerializeField] private GameObject platformSpacePrefab;

        [Header("Number of Platforms Allowed")]
        [SerializeField] private int maxPlatformCount;
        
        [Header("Platform Spawn Points")]
        [SerializeField] private List<Vector3> platformLocationList;

        [Header("Creep Navigation Nodes")]
        [SerializeField] private List<GameObject> nodeList;
        
        [Header("Active Platform and Platform Spaces")]
        public List<GameObject> platformSpaceList;
        public List<GameObject> platformList;

        // Start is called before the first frame update
        private void Start()
        {
            InitializtionChecks();

            foreach (var spawnLocation in platformLocationList)
            {
                var newPlatformSpace = Instantiate(platformSpacePrefab, spawnLocation,
                    platformSpacePrefab.transform.rotation);
                platformSpaceList.Add(newPlatformSpace);
            }
        }

        /// <summary>
        ///     Verifies all required variables are valid
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when a variable is null or otherwise invalid</exception>
        private void InitializtionChecks()
        {
            if (platformSpacePrefab == null) throw new ArgumentNullException("platformSpacePrefab is null");

            if (platformPrefab == null) throw new ArgumentNullException("platformPrefab is null");

            if (nodeList == null) throw new ArgumentNullException("nodeList is null");

            if (nodeList.Count < 2) throw new ArgumentNullException("nodeList should have at least 2 nodes");

            if (platformLocationList == null) throw new ArgumentNullException("platformLocationList is null");

            if (platformLocationList.Count < 2)
                throw new ArgumentNullException("platformLocationList should have at least 2 items");
        }

        /// <summary>
        ///     Gets the max platform amount for this map
        /// </summary>
        /// <returns> Max platform amount as an int </returns>
        public int GetMaxPlatformCount()
        {
            return maxPlatformCount;
        }

        /// <summary>
        ///     Gets the list of the creep navigation nodes for this map
        /// </summary>
        /// <returns> List of navigation node GameObjects </returns>
        public List<GameObject> GetNodeList()
        {
            return nodeList;
        }

        /// <summary>
        ///     Gets the platform prefab for this map
        /// </summary>
        /// <returns> Prefab GameObject for the platform </returns>
        public GameObject GetPlatformPrefab()
        {
            return platformPrefab;
        }

        /// <summary>
        ///     Gets a dictionary containing the towers currently active in the game
        ///     Key is a tuple, with the first int being the platform number and the second being the socket number
        /// </summary>
        /// <returns> Dictionary containing tower GameObjects in play </returns>
        public Dictionary<Tuple<int, int>, GameObject> GetActiveTowers()
        {
            Dictionary<Tuple<int, int>, GameObject> activeTowers = new Dictionary<Tuple<int, int>, GameObject>();

            for (var platformIndex = 0; platformIndex < platformList.Count; platformIndex++)
            {
                GameObject platform = platformList[platformIndex];
                
                for (var socketIndex = 0; socketIndex < platform.transform.childCount; socketIndex++)
                {
                    GameObject socket = platform.transform.GetChild(socketIndex).gameObject;

                    if (socket.transform.childCount > 0)
                    {
                        GameObject tower = socket.transform.GetChild(0).gameObject;
                        activeTowers.Add(new Tuple<int, int>(platformIndex, socketIndex), tower);
                    }
                }
            }

            return activeTowers;
        }

        /// <summary>
        ///     Gets a tower GameObject by it's platform and socket
        /// </summary>
        /// <param name="platformIndex"> Platform number the tower is built on </param>
        /// <param name="socketIndex"> Socket on the platform the tower is built on </param>
        /// <returns> Tower GameObject </returns>
        public GameObject GetTowerByIndex(int platformIndex, int socketIndex)
        {
            Dictionary<Tuple<int, int>, GameObject> activeTowers = GetActiveTowers();
            Tuple<int, int> index = new Tuple<int, int>(platformIndex, socketIndex);

            if (activeTowers.ContainsKey(index))
            {
                return activeTowers[index];
            }

            return null;
        }
    }
}