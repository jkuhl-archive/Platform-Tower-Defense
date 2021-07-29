using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Menus
{
    public class MainMenuLogic : MonoBehaviour
    {
        [Header("Main Menu Text GameObjects")]
        [SerializeField] private GameObject gameTitleText;

        [SerializeField] private GameObject developerNameText;
        [SerializeField] private GameObject gameVersionText;

        [Header("Settings Menu GameObjects")]
        [SerializeField] private GameObject resolutionSelectDropdown;

        [Header("Menu Page Canvas GameObjects")]
        [SerializeField] private List<GameObject> menuPageCanvasList;

        [Header("Menu Sound Effects")]
        [SerializeField] private AudioClip buttonClickSoundEffect;
        
        /// <summary>
        ///     Enum for storing possible main menu states
        /// </summary>
        private enum MainMenuState
        {
            Main,
            GameModeSelect,
            MapSelect,
            Settings,
            Null
        }

        // Menu state variables
        private MainMenuState currentMenuState = MainMenuState.Null;

        // Start is called before the first frame update
        private void Start()
        {
            // Initialize menu
            InitializeGameInfoTextObjects();
            InitializeSettingsMenuObjects();
            ChangeMenuState(MainMenuState.Main);

            // Set saved player preferences
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(
                PlayerPrefsUtils.GetBool(PlayerPrefsConstants.KeySoundEnabled,
                    PlayerPrefsConstants.DefaultSoundEnabled));
        }

        /// <summary>
        ///     Switches the active menu page
        /// </summary>
        /// <param name="newMenuState"> MainMenuState that we want to switch to </param>
        private void ChangeMenuState(MainMenuState newMenuState)
        {
            if (currentMenuState == newMenuState) return;

            foreach (var menuPageCanvas in menuPageCanvasList)
            {
                menuPageCanvas.SetActive(false);

                if (menuPageCanvas.name == $"{newMenuState.ToString()}Canvas")
                {
                    Debug.Log($"Changing menu state to new state: '{newMenuState.ToString()}'");

                    menuPageCanvas.SetActive(true);
                    currentMenuState = newMenuState;
                }
            }

            if (currentMenuState != newMenuState)
                Debug.Log($"Failed to change menu state to new state: '{newMenuState.ToString()}'");
        }

        /// <summary>
        ///     Loads application name, developer, and version from Unity's build settings and sets their values in the UI
        /// </summary>
        private void InitializeGameInfoTextObjects()
        {
            gameTitleText.GetComponent<Text>().text = Application.productName;
            developerNameText.GetComponent<Text>().text = $"(C) {Application.companyName}";

            var versionString = $"Version {Application.version}";
            if (Application.isEditor) versionString = $"Version {Application.version} Debug";

            gameVersionText.GetComponent<Text>().text = $"{versionString}\n" +
                                                        $"Unity Version {Application.unityVersion}";
        }

        /// <summary>
        ///     Initializes objects in the settings menu such as the resolution drop down menu
        /// </summary>
        private void InitializeSettingsMenuObjects()
        {
            // Populate screen resolution drop down menu
            var resolutionList = resolutionSelectDropdown.GetComponent<Dropdown>().options;
            foreach (var pair in SupportedResolutions.AvailableResolutions)
                resolutionList.Add(new Dropdown.OptionData(pair.Value.ToString()));

            // TODO: Fix issue with getting refresh rates so we don't have to hard code 60 here
            var resolutionKey = SupportedResolutions.GetResolutionKey(Screen.height, Screen.width, 60);
            resolutionSelectDropdown.GetComponent<Dropdown>().value = resolutionKey;
        }

        /// <summary>
        ///     Attempts to store the selected map's prefabs in the buffer and then start the game
        /// </summary>
        /// <param name="mapName"> Name of the selected map </param>
        private void SelectGameMap(string mapName)
        {
            var success = InitializationBuffer.SelectGameMap(mapName);

            if (success) StartGame();
        }

        /// <summary>
        ///     Attempts to store the selected game mode's prefabs in the buffer and then switch to the map select menu
        /// </summary>
        /// <param name="gameModeName"> Name of the selected game mode </param>
        private void SelectGameMode(string gameModeName)
        {
            var success = InitializationBuffer.SelectGameMode(gameModeName);

            if (success) ChangeMenuState(MainMenuState.MapSelect);
        }

        /// <summary>
        ///     Attempts to leave the menu and start gameplay
        /// </summary>
        private void StartGame()
        {
            if (!InitializationBuffer.BufferReady())
            {
                Debug.Log("Not ready to start game");
                return;
            }

            GameUtils.GetGameLoadingLogic().LoadGameplay();
        }

        /// <summary>
        ///     Handles menu button actions
        /// </summary>
        /// <param name="buttonGameObject"> GameObject for the menu button that was selected </param>
        public void MenuButtonHandler(GameObject buttonGameObject)
        {
            Debug.Log($"Clicked menu button: '{buttonGameObject.name}'");
            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(buttonClickSoundEffect);

            switch (buttonGameObject.name)
            {
                // Main menu buttons
                case "PlayGameButton":
                case "BackToGameModeSelectButton":
                    InitializationBuffer.ResetBuffer();
                    ChangeMenuState(MainMenuState.GameModeSelect);
                    break;
                case "SettingsButton":
                    ChangeMenuState(MainMenuState.Settings);
                    break;
                case "ExitGameButton":
                    GameUtils.ExitGame();
                    break;

                // Settings menu buttons
                case "ToggleFullScreenButton":
                    GameSettingsUtils.ToggleFullScreen();
                    break;
                case "ToggleSoundButton":
                    GameSettingsUtils.ToggleSound();
                    break;
                case "BackToMainMenuButton":
                    ChangeMenuState(MainMenuState.Main);
                    break;

                // Game mode menu buttons
                case "ClassicTDModeButton":
                    SelectGameMode("ClassicTD");
                    break;

                // Game map menu buttons
                case "VillageMapButton":
                    SelectGameMap("Village");
                    break;
            }
        }

        /// <summary>
        ///     Handles interactions with the resolution selection drop down menu in the settings menu
        /// </summary>
        /// <param name="selectedResolution"> Dropdown object returned by the UI when interacted with </param>
        public void ResolutionSelectHandler(Dropdown selectedResolution)
        {
            GameSettingsUtils.SetGameResolution(selectedResolution.value);
        }
    }
}