using UnityEngine;

namespace Utilities
{
    public static class PlayerDataUtils
    {
        // All interfacing with PlayerPrefs should be done using static values stored here to keep things in one place
        public static string key_fullScreenEnabled = "full_screen_enabled";
        public static string key_soundEnabled = "sound_enabled";
        
        public static bool default_fullScreenEnabled = false;
        public static bool default_soundEnabled = true;
    
        /// <summary>
        /// Gets a boolean value stored in the PlayerPrefs
        /// Stores the default value in the PlayerPrefs if the key does not exist
        /// </summary>
        /// <param name="keyName"> Name of the boolean we want to retrieve </param>
        /// <param name="defaultValue"> Value to be returned if there is no value for the given key </param>
        /// <returns> Boolean representing the value in the PlayerPrefs </returns>
        public static bool GetBool(string keyName, bool defaultValue)
        {
            if (PlayerPrefs.HasKey(keyName))
            {
                return PlayerPrefs.GetInt(keyName) != 0;
            }
        
            SetBool(keyName, defaultValue);

            return defaultValue;
        }

        /// <summary>
        /// Sets a boolean value in the PlayerPrefs
        /// </summary>
        /// <param name="keyName"> Name of the boolean we want to set </param>
        /// <param name="value"> Value we want to set the boolean to </param>
        public static void SetBool(string keyName, bool value)
        {
            PlayerPrefs.SetInt(keyName, value ? 1 : 0);
        }

        /// <summary>
        /// Toggles a boolean value in the PlayerPrefs
        /// </summary>
        /// <param name="keyName"> Name of the boolean we want to retrieve </param>
        /// <param name="defaultValue"> Value to be returned if there is no value for the given key </param>
        /// <returns> Boolean representing the value in the PlayerPrefs after being inverted </returns>
        public static bool ToggleBool(string keyName, bool defaultValue)
        {
            bool newValue = defaultValue;
        
            if (PlayerPrefs.HasKey(keyName))
            {
                newValue = !GetBool(keyName, defaultValue);
            }
        
            SetBool(keyName, newValue);
            return newValue;
        }

        /// <summary>
        /// Checks if the game's sound is enabled
        /// </summary>
        /// <returns> Returns true if the audio is enabled, false if not</returns>
        public static bool IsSoundEnabled()
        {
            return GetBool(key_soundEnabled, true);
        }
    }
}
