using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Gameplay.Waves
{
    public class WaveManager : MonoBehaviour
    {
        [Header("WavePlaylist JSON File")] 
        public string wavePlaylistJson;
        
        [Header("Creep Prefabs")]
        public GameObject gruntPrefab;
        public GameObject fastGruntPrefab;
        public GameObject tankGruntPrefab;

        [Header("Wave UI GameObjects")]
        public GameObject waveCounterText;
        public GameObject waveInfoText;

        [Header("Active Creeps")]
        public List<GameObject> creepList;

        // Wave data used for spawning creeps
        private WavePlaylist wavePlaylist;
        
        // Dictionary that stores a list of creeps that were spawned in each round, creeps are removed as they die
        private Dictionary<int, List<GameObject>> waveActiveCreeps = new Dictionary<int, List<GameObject>>();

        // Creep spawn point
        private GameObject startPoint;
        
        // Gameplay related variables
        private bool readyToStart;

        // Start is called before the first frame update
        void Start()
        {
            wavePlaylist = WavePlaylist.FromJson(wavePlaylistJson);
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsReady())
            {
                SetStartPoint();
                return;
            }

            if (GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().IsGameActive())
            {
                Execute();
                UpdateTextObjects();
            }
        }

        /// <summary>
        /// Handles starting and stopping of waves and spawning creeps
        /// </summary>
        void Execute()
        {
            // If the game is not active, return immediately
            if (!GameUtils.GetRootGameObjectByName("GameLogic").GetComponent<PlayerLogic>().IsGameActive())
            {
                return;
            }

            // Attempt to spawn a creep
            if (wavePlaylist.CanSpawnCreep())
            {
                WaveSpawnObject nextSpawn = wavePlaylist.GetNextSpawn();
                GameObject creepPrefab = GetCreepPrefabByName(nextSpawn.creepName);
                
                if (creepPrefab == null)
                {
                    Debug.Log($"Unable to locate prefab for creep: '{nextSpawn.creepName}'");
                    return;
                }
                
                // Spawn creep and add it to the creep list
                GameObject newCreep = Instantiate(creepPrefab, startPoint.transform.position, startPoint.transform.rotation);
                creepList.Add(newCreep);

                // Add newly spawned creep to list of creeps spawned during this wave
                if (!waveActiveCreeps.ContainsKey(wavePlaylist.GetCurrentWaveNumber()))
                {
                    waveActiveCreeps.Add(wavePlaylist.GetCurrentWaveNumber(), new List<GameObject>());
                }
                waveActiveCreeps[wavePlaylist.GetCurrentWaveNumber()].Add(newCreep);
            }

            // Update lists of active creeps for each round to monitor when a wave reward should be given
            foreach (int key in waveActiveCreeps.Keys.ToArray())
            {
                // Stop storing creeps that have been destroyed already
                foreach (GameObject creep in waveActiveCreeps[key].ToArray())
                {
                    if (!creepList.Contains(creep))
                    {
                        waveActiveCreeps[key].Remove(creep);
                    }
                }

                // If a wave is finished and all of it's creeps are destroyed,
                // give the player a reward and stop monitoring the wave
                if (wavePlaylist.waveDataList[key - 1].WaveFinished())
                {
                    if (waveActiveCreeps[key].Count <= 0 && !wavePlaylist.waveDataList[key - 1].RewardGiven())
                    {
                        Debug.Log($"Wave {key} complete, giving player " +
                                  $"${wavePlaylist.waveDataList[key - 1].waveReward} reward");
                        wavePlaylist.waveDataList[key - 1].GiveReward();
                        waveActiveCreeps.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a creep prefab using the given name string
        /// </summary>
        /// <param name="prefabName"> Name of the creep prefab we want to get </param>
        /// <returns> GameObject containing the creep prefab </returns>
        GameObject GetCreepPrefabByName(string prefabName)
        {
            switch (prefabName)
            {
                case "Grunt":
                    return gruntPrefab;
                case "FastGrunt":
                    return fastGruntPrefab;
                case "TankGrunt":
                    return tankGruntPrefab;
                
                default:
                    return null;
            }
        }

        /// <summary>
        /// Attempts to pull the creep spawn point from Map, if successful marks the object as ready to start
        /// </summary>
        void SetStartPoint()
        {
            GameObject gameMap = GameUtils.GetRootGameObjectByName("Map");

            if (gameMap != null)
            {
                startPoint = gameMap.GetComponent<MapLogic>().nodeList[0];
                readyToStart = true;
                wavePlaylist.StartSpawning();
            }
        }
    
        /// <summary>
        /// Updates the text of all UI elements
        /// </summary>
        void UpdateTextObjects()
        {
            // Attempt to update wave count text
            string waveCounterString = $"Wave: {wavePlaylist.GetCurrentWaveNumber()}";
            if (waveCounterText.GetComponent<Text>().text != waveCounterString)
            {
                waveCounterText.GetComponent<Text>().text = waveCounterString;
            }
        
            // Attempt to update wave info text
            if (waveInfoText.GetComponent<Text>().text != wavePlaylist.GetWaveStatusMessage())
            {
                waveInfoText.GetComponent<Text>().text = wavePlaylist.GetWaveStatusMessage();
            }
        }

        /// <summary>
        /// Checks if this object is ready for the game to start
        /// </summary>
        /// <returns> True if all required values are initialized, false if not </returns>
        public bool IsReady()
        {
            return readyToStart;
        }
    }
}
