using Data;
using UnityEngine;

namespace Utilities
{
    public static class GameSettingsUtils
    {
        /// <summary>
        ///     Checks if the game's sound is enabled
        /// </summary>
        /// <returns> Returns true if the audio is enabled, false if not</returns>
        public static bool IsSoundEnabled()
        {
            return PlayerPrefsUtils.GetBool(PlayerPrefsConstants.KeySoundEnabled,
                PlayerPrefsConstants.DefaultSoundEnabled);
        }

        /// <summary>
        ///     Applies a resolution from the AvailableResolutions dictionary
        /// </summary>
        /// <param name="selectedResolutionKey"> Int key used to retrieve the resolution from the AvailableResolution dictionary </param>
        /// <returns> True if able to successfully set the resolution, false if not </returns>
        public static bool SetGameResolution(int selectedResolutionKey)
        {
            var selectedResolution = SupportedResolutions.AvailableResolutions[selectedResolutionKey];

            if (Screen.height != selectedResolution.height ||
                Screen.width != selectedResolution.width ||
                Screen.currentResolution.refreshRate != selectedResolution.refreshRate)
            {
                Screen.SetResolution(height: selectedResolution.height,
                    width: selectedResolution.width,
                    fullscreen: PlayerPrefsUtils.GetBool(PlayerPrefsConstants.KeyFullScreenEnabled,
                        PlayerPrefsConstants.DefaultFullScreenEnabled),
                    preferredRefreshRate: selectedResolution.refreshRate);

                return true;
            }

            return false;
        }

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
    }
}