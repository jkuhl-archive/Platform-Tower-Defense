using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

namespace Gameplay.WaveLogic
{
    [Serializable]
    public class WavePlaylist
    {
        // WavePlaylist JSON file directory
        private static readonly string JsonDirectory = Path.Combine("Json", "WavePlaylists");

        // WavePlaylist data
        public int timeBetweenWaves;
        public List<WaveData> waveDataList;
        private float nextSpawnTime;
        private float nextWaveTime;

        // WavePlaylist status variables
        private bool spawningActive;
        private bool waveActive;
        private int waveDataIndex;

        /// <summary>
        ///     Object for storing a 'playlist' of waves for the player to complete
        /// </summary>
        /// <param name="timeBetweenWaves"> Amount of time in seconds that should pass before a new wave starts </param>
        /// <param name="waveDataList"> List of WaveData objects containing data for individual waves of creeps</param>
        public WavePlaylist(int timeBetweenWaves, List<WaveData> waveDataList)
        {
            this.timeBetweenWaves = timeBetweenWaves;
            this.waveDataList = waveDataList;

            spawningActive = false;
            waveActive = false;
            waveDataIndex = 0;
            nextWaveTime = 0;
            nextSpawnTime = 0;
        }

        /// <summary>
        ///     Checks if the game is ready for a creep to be spawned
        /// </summary>
        /// <returns> True if ready to spawn a new creep, false if not </returns>
        public bool CanSpawnCreep()
        {
            // If spawning is disabled, return false
            if (!spawningActive) return false;

            // All waves has been completed, disable spawning and return false
            if (!waveDataList[waveDataIndex].HasAvailableSpawn() && waveDataIndex >= waveDataList.Count - 1)
            {
                spawningActive = false;
                return false;
            }

            // Jump to the next wave if the current wave has no more creeps to spawn and there are additional waves
            if (!waveDataList[waveDataIndex].HasAvailableSpawn() && waveDataIndex < waveDataList.Count - 1)
            {
                waveDataIndex++;
                nextWaveTime = Time.time + timeBetweenWaves;
                waveActive = false;
                return false;
            }

            // If we are not currently in a wave but are ready to start one, kick it off
            if (!waveActive)
                if (Time.time > nextWaveTime)
                {
                    Debug.Log($"Starting wave {GetCurrentWaveNumber()}");
                    waveActive = true;
                }

            // If we are in a wave and ready to spawn a creep, return the value of the waves 'HasAvailabelSpawn'
            if (waveActive && Time.time > nextSpawnTime) return waveDataList[waveDataIndex].HasAvailableSpawn();

            return false;
        }

        /// <summary>
        ///     Gets the current wave number
        /// </summary>
        /// <returns> Value of waveDataIndex incremented by 1 to account for list indexes starting with 0 </returns>
        public int GetCurrentWaveNumber()
        {
            return waveDataIndex + 1;
        }

        /// <summary>
        ///     Gets the WaveSpawnObject for the next creep that will be spawned
        /// </summary>
        /// <returns> WaveSpawnObject containing variables about the creep to be spawned </returns>
        public WaveSpawnObject GetNextSpawn()
        {
            if (waveDataList[waveDataIndex].HasAvailableSpawn())
            {
                var nextSpawn = waveDataList[waveDataIndex].GetNextSpawn();
                nextSpawnTime = Time.time + nextSpawn.timeBeforeNextSpawn;

                return nextSpawn;
            }

            return null;
        }

        /// <summary>
        ///     Gets the status message that should be displayed
        /// </summary>
        /// <returns> Wave status message as a string </returns>
        public string GetWaveStatusMessage()
        {
            if (waveDataList[waveDataList.Count - 1].RewardGiven()) return "All waves complete!";

            if (waveActive) return waveDataList[waveDataIndex].waveMessage;

            if (!waveActive)
            {
                var nextWaveCountdown = (int) (nextWaveTime - Time.time);
                var nextWaveMessage = $"starting in {nextWaveCountdown} seconds";

                if (nextWaveCountdown == 0) return "Wave starting now!";

                if (nextWaveCountdown == 1) nextWaveMessage = $"starting in {nextWaveCountdown} second";

                if (waveDataIndex == 0) return $"Game {nextWaveMessage}";

                return $"Next wave {nextWaveMessage}";
            }

            return "";
        }

        /// <summary>
        ///     Begins 'playing' the playlist and starts spawning creeps
        /// </summary>
        public void StartSpawning()
        {
            spawningActive = true;
            nextWaveTime = Time.time + timeBetweenWaves + 6;
        }
        
        /// <summary>
        ///     Dumps the data in this WavePlaylist to a JSON file
        /// </summary>
        /// <param name="jsonFileName"> Name of the JSON file we want to write data to, should look like: "test.json" </param>
        public void ToJson(string jsonFileName)
        {
            var filePath = Path.Combine(Application.dataPath, JsonDirectory, jsonFileName);

            Debug.Log($"Writing WavePlaylist data to JSON file: '{filePath}'");

            var writer = new StreamWriter(filePath);
            writer.WriteLine(JsonUtility.ToJson(this, true));
            writer.Close();
        }
        
        /// <summary>
        ///     Loads a WavePlaylist object from the given JSON file
        /// </summary>
        /// <param name="jsonFileName"> Name of the JSON file we want to load data from, should look like: "test.json" </param>
        /// <returns> WavePlaylist object containing data loaded from the JSON file </returns>
        public static WavePlaylist FromJson(string jsonFileName)
        {
            var filePath = Path.Combine(JsonDirectory, jsonFileName);

            if (GameUtils.ResourceFileExists(filePath, "json"))
            {
                Debug.Log($"Loading WavePlaylist data from JSON file: '{filePath}'");
                var jsonText = Resources.Load<TextAsset>(filePath);
                return JsonUtility.FromJson<WavePlaylist>(jsonText.ToString());
            }
            else
            {
                Debug.Log($"WavePlaylist JSON file does not exist: '{filePath}'");
            }
            
            Debug.Log($"Failed to load WavePlaylist from JSON file: '{filePath}'");
            return null;
        }
    }
}