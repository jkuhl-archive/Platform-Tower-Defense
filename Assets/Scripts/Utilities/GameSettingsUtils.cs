using Data;
using UnityEngine;

namespace Utilities
{
    public static class GameSettingsUtils
    {
        /// <summary>
        ///     Enables or disables full screen mode
        /// </summary>
        public static void ToggleFullScreen()
        {
            var b = PlayerPrefsUtils.ToggleBool(PlayerPrefsConstants.KeyFullScreenEnabled,
                PlayerPrefsConstants.DefaultFullScreenEnabled);
            Screen.fullScreen = b;
        }

        /// <summary>
        ///     Enables or disables all sound in the game
        /// </summary>
        public static void ToggleSound()
        {
            var b = PlayerPrefsUtils.ToggleBool(PlayerPrefsConstants.KeySoundEnabled,
                PlayerPrefsConstants.DefaultSoundEnabled);
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(b);
        }

        /// <summary>
        ///     Checks if the game's sound is enabled
        /// </summary>
        /// <returns> Returns true if the audio is enabled, false if not</returns>
        public static bool IsSoundEnabled()
        {
            return PlayerPrefsUtils.GetBool(PlayerPrefsConstants.KeySoundEnabled, true);
        }
    }
}