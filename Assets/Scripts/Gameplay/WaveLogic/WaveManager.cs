using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Gameplay.WaveLogic
{
    public class WaveManager : MonoBehaviour
    {
        [Header("WavePlaylist JSON File")]
        [SerializeField] private string wavePlaylistName;

        [Header("Creep Prefabs")]
        [SerializeField] private GameObject gruntPrefab;
        [SerializeField] private GameObject fastGruntPrefab;
        [SerializeField] private GameObject tankGruntPrefab;

        [Header("Active Creeps")]
        public List<GameObject> creepList;

        // Creep spawn point
        private GameObject startPoint;

        // Dictionary that stores a list of creeps that were spawned in each round, creeps are removed as they die
        private readonly Dictionary<int, List<GameObject>> waveActiveCreeps = new Dictionary<int, List<GameObject>>();

        // Wave data used for spawning creeps
        private WavePlaylist wavePlaylist;

        // Start is called before the first frame update
        private void Start()
        {
            wavePlaylist = WavePlaylist.FromJson(wavePlaylistName);
            startPoint = GameUtils.GetMapLogic().GetNodeList()[0];
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused()) Execute();
        }

        /// <summary>
        ///     Handles starting and stopping of waves and spawning creeps
        /// </summary>
        private void Execute()
        {
            // Return immediately if no WavePlaylist is loaded
            if (wavePlaylist == null)
            {
                return;
            }

            // Attempt to spawn a creep
            if (wavePlaylist.CanSpawnCreep())
            {
                var nextSpawn = wavePlaylist.GetNextSpawn();
                var creepPrefab = GetCreepPrefabByName(nextSpawn.creepName);

                if (creepPrefab == null)
                {
                    Debug.Log($"Unable to locate prefab for creep: '{nextSpawn.creepName}'");
                    return;
                }

                // Spawn creep and add it to the creep list
                var newCreep = Instantiate(creepPrefab, startPoint.transform.position, startPoint.transform.rotation);
                creepList.Add(newCreep);

                // Add newly spawned creep to list of creeps spawned during this wave
                if (!waveActiveCreeps.ContainsKey(wavePlaylist.GetCurrentWaveNumber()))
                    waveActiveCreeps.Add(wavePlaylist.GetCurrentWaveNumber(), new List<GameObject>());
                waveActiveCreeps[wavePlaylist.GetCurrentWaveNumber()].Add(newCreep);
            }

            // Update lists of active creeps for each round to monitor when a wave reward should be given
            foreach (var key in waveActiveCreeps.Keys.ToArray())
            {
                // Stop storing creeps that have been destroyed already
                foreach (var creep in waveActiveCreeps[key].ToArray())
                    if (!creepList.Contains(creep))
                        waveActiveCreeps[key].Remove(creep);

                // If a wave is finished and all of it's creeps are destroyed,
                // give the player a reward and stop monitoring the wave
                if (wavePlaylist.waveDataList[key - 1].WaveFinished())
                    if (waveActiveCreeps[key].Count <= 0 && !wavePlaylist.waveDataList[key - 1].RewardGiven())
                    {
                        Debug.Log($"Wave {key} complete, giving player " +
                                  $"${wavePlaylist.waveDataList[key - 1].waveReward} reward");
                        wavePlaylist.waveDataList[key - 1].GiveReward();
                        waveActiveCreeps.Remove(key);
                    }
            }
        }

        /// <summary>
        ///     Gets a creep prefab using the given name string
        /// </summary>
        /// <param name="prefabName"> Name of the creep prefab we want to get </param>
        /// <returns> GameObject containing the creep prefab </returns>
        private GameObject GetCreepPrefabByName(string prefabName)
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
        /// Gets the current wave number
        /// </summary>
        /// <returns> Value of waveDataIndex incremented by 1 to account for list indexes starting with 0 </returns>
        public int GetCurrentWaveNumber()
        {
            if (wavePlaylist == null)
            {
                return 0;
            }
            
            return wavePlaylist.GetCurrentWaveNumber();
        }
        
        /// <summary>
        /// Gets the status message that should be displayed
        /// </summary>
        /// <returns> Wave status message as a string </returns>
        public string GetWaveStatusMessage()
        {
            if (wavePlaylist == null)
            {
                return String.Empty;
            }
            
            return wavePlaylist.GetWaveStatusMessage();
        }

        /// <summary>
        /// Begins 'playing' the playlist and starts spawning creeps
        /// </summary>
        public bool StartSpawning()
        {
            if (wavePlaylist == null)
            {
                return false;
            }
            
            wavePlaylist.StartSpawning();
            return true;
        }
    }
}