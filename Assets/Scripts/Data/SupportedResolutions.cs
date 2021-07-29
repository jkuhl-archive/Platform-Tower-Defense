using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public static class SupportedResolutions
    {
        // Dictionary that stores all resolutions supported by the current display with an int as a key
        public static readonly Dictionary<int, Resolution> AvailableResolutions = GenerateMonitorResolutionDictionary();

        /// <summary>
        ///     Compiles the resolutions supported by the current monitor into a dictionary for ease of repeated access
        /// </summary>
        /// <returns> Dictionary containing resolutions </returns>
        private static Dictionary<int, Resolution> GenerateMonitorResolutionDictionary()
        {
            // TODO: Determine why this method only returns 60hz refresh rates
            var monitorResolutions = Screen.resolutions;
            var resolutionDictionary = new Dictionary<int, Resolution>();

            for (var i = 0; i < monitorResolutions.Length; i++) resolutionDictionary.Add(i, monitorResolutions[i]);

            return resolutionDictionary;
        }

        /// <summary>
        ///     Attempts to find a resolution in the AvailableResolutions dictionary
        /// </summary>
        /// <param name="height"> Height of the resolution we are looking for </param>
        /// <param name="width"> Width of the resolution we are looking for </param>
        /// <param name="refreshRate"> Refresh rate of the resolution we are looking for </param>
        /// <returns> Int key used to retrieve the resolution from the AvailableResolution dictionary </returns>
        public static int GetResolutionKey(int height, int width, int refreshRate)
        {
            foreach (var resolution in AvailableResolutions)
                if (height == resolution.Value.height &&
                    width == resolution.Value.width &&
                    refreshRate == resolution.Value.refreshRate)
                    return resolution.Key;

            return 0;
        }
    }
}