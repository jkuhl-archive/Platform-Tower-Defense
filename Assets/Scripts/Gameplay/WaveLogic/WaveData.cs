using System;
using System.Collections.Generic;
using Utilities;

namespace Gameplay.WaveLogic
{
    [Serializable]
    public class WaveData
    {
        // Wave specific variables
        public string waveMessage;
        public int waveReward;
        public List<WaveSpawnObject> waveSpawns;

        // Wave progress tracking variables
        private int spawnIndex;
        private bool waveFinished;
        private bool rewardGiven;

        /// <summary>
        ///     Object for storing all of the data that describes how an single wave of creeps should be generated
        /// </summary>
        /// <param name="waveMessage"> Message to be displayed in game while the wave is being spawned </param>
        /// <param name="waveReward"> Amount of money to give the player after they complete the wave </param>
        /// <param name="waveSpawns"> List of WaveSpawnObject items, one for each creep to be spawned </param>
        public WaveData(string waveMessage, int waveReward, List<WaveSpawnObject> waveSpawns)
        {
            this.waveMessage = waveMessage;
            this.waveReward = waveReward;
            this.waveSpawns = waveSpawns;

            spawnIndex = 0;
        }
        
        /// <summary>
        ///     Gets the WaveSpawnObject for the next creep that will be spawned
        /// </summary>
        /// <returns> WaveSpawnObject containing variables about the creep to be spawned </returns>
        public WaveSpawnObject GetNextSpawn()
        {
            if (HasAvailableSpawn())
            {
                var nextSpawn = waveSpawns[spawnIndex];
                spawnIndex++;

                // If all creeps in this wave been spawned, the wave is finished
                if (spawnIndex >= waveSpawns.Count) waveFinished = true;

                return nextSpawn;
            }

            return null;
        }
        
        /// <summary>
        ///     Adds the wave's reward amount to the player's money
        /// </summary>
        public void GiveReward()
        {
            if (!RewardGiven())
            {
                rewardGiven = true;
                GameUtils.GetPlayerLogic().UpdatePlayerMoney(waveReward);
            }
        }

        /// <summary>
        ///     Checks if there are any creeps left in this wave that can be spawned
        /// </summary>
        /// <returns> True if there are remaining creeps in the wave, false if not </returns>
        public bool HasAvailableSpawn()
        {
            if (spawnIndex < waveSpawns.Count) return true;

            return false;
        }
        
        /// <summary>
        ///     Checks if the player has been given a reward for finishing the wave
        /// </summary>
        /// <returns> Returns true if the player has been given a reward, false if not </returns>
        public bool RewardGiven()
        {
            return rewardGiven;
        }

        /// <summary>
        ///     Checks if the wave is completed
        /// </summary>
        /// <returns> Returns true if the wave is finished, false if not </returns>
        public bool WaveFinished()
        {
            return waveFinished;
        }
    }
}