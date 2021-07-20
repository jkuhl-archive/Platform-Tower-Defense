using System;

namespace Gameplay.Waves
{
    [Serializable]
    public class WaveSpawnObject
    {
        public string creepName;
        public float timeBeforeNextSpawn;

        /// <summary>
        /// Object for storing data about a single creep that will be spawned
        /// </summary>
        /// <param name="creepName"> Name of the creep prefab we want to spawn </param>
        /// <param name="timeBeforeNextSpawn"> Amount of time in seconds to wait before spawning the next creep </param>
        public WaveSpawnObject(string creepName, float timeBeforeNextSpawn)
        {
            this.creepName = creepName;
            this.timeBeforeNextSpawn = timeBeforeNextSpawn;
        }
    }
}