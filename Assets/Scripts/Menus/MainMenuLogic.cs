using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Menus
{
    /// <summary>
    /// Enum for storing possible main menu states
    /// </summary>
    public enum MainMenuState
    {
        Main,
        GameModeSelect,
        MapSelect,
        Settings,
        Null
    }
    
    public class MainMenuLogic : MonoBehaviour
    {
        [Header("Main Menu Text GameObjects")]
        public GameObject gameTitleText;
        public GameObject developerNameText;
        public GameObject gameVersionText;

        [Header("Menu Page Canvas GameObjects")]
        public List<GameObject> menuPageCanvasList;
        
        [Header("Game Map GameObjects")]
        public GameObject villageMapPrefab;
        
        [Header("Menu Sound Effects")]
        public AudioClip buttonClickSoundEffect;
        
        // Menu state variables
        private MainMenuState currentMenuState = MainMenuState.Null;
        private string selectedGameModeSceneName = null;

        // Start is called before the first frame update
        void Start()
        {
            InitializeMenuTextObjects();
            ChangeMenuState(MainMenuState.Main);
            
            // Set saved player preferences
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(PlayerDataUtils.GetBool(PlayerDataUtils.key_soundEnabled, PlayerDataUtils.default_soundEnabled));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Loads application name, developer, and version from Unity's build settings and sets their values in the UI
        /// </summary>
        void InitializeMenuTextObjects()
        {
            gameTitleText.GetComponent<Text>().text = Application.productName;
            developerNameText.GetComponent<Text>().text = $"(C) {Application.companyName}";

            string versionString = $"Version {Application.version}";
            if (Application.isEditor)
            {
                versionString = $"Version {Application.version} Debug";
            }
            
            gameVersionText.GetComponent<Text>().text = $"{versionString}\n" +
                                                        $"Unity Version {Application.unityVersion}";
        }
        
        /// <summary>
        /// Switches the active menu page
        /// </summary>
        /// <param name="newMenuState"> MainMenuState that we want to switch to </param>
        void ChangeMenuState(MainMenuState newMenuState)
        {
            if (currentMenuState == newMenuState)
            {
                return;
            }
            
            foreach (GameObject menuPageCanvas in menuPageCanvasList)
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
            {
                Debug.Log($"Failed to change menu state to new state: '{newMenuState.ToString()}'");
            }
        }

        /// <summary>
        /// Exits the game
        /// </summary>
        void ExitGame()
        {
            if (Application.isEditor)
            {
                return;
            }
            
            Application.Quit(0);
        }

        /// <summary>
        /// Stores the selectedGameModeSceneName variable and then switches to the MapSelect screen
        /// </summary>
        /// <param name="gameModeSceneName"> Name of the selected game mode's Unity scene </param>
        void SelectGameMode(string gameModeSceneName)
        {
            if (selectedGameModeSceneName == null)
            {
                Debug.Log($"Selected game mode: '{gameModeSceneName}'");
                selectedGameModeSceneName = gameModeSceneName;
                
            }
            
            if (selectedGameModeSceneName != null)
            {
                ChangeMenuState(MainMenuState.MapSelect);
            }
        }

        /// <summary>
        /// Attempts to leave the menu and start gameplay
        /// </summary>
        /// <param name="mapPrefab"> Game map prefab that we want to load once the game scene is loaded </param>
        void StartGame(GameObject mapPrefab)
        {
            if (selectedGameModeSceneName != null)
            {
                GameUtils.StartNewGame(selectedGameModeSceneName, mapPrefab);
            }
        }

        /// <summary>
        /// Enables or disables full screen mode
        /// </summary>
        void ToggleFullScreen()
        {
            bool newValue = PlayerDataUtils.ToggleBool(PlayerDataUtils.key_fullScreenEnabled,
                PlayerDataUtils.default_fullScreenEnabled);

            Screen.fullScreen = newValue;
        }

        /// <summary>
        /// Enables or disables all sound in the game
        /// </summary>
        void ToggleSound()
        {
            bool b = PlayerDataUtils.ToggleBool(PlayerDataUtils.key_soundEnabled, PlayerDataUtils.default_soundEnabled);
            GameUtils.GetRootGameObjectByName("BackgroundMusic").SetActive(b);
        }

        /// <summary>
        /// Handles menu button actions
        /// </summary>
        /// <param name="buttonGameObject"> GameObject for the menu button that was selected </param>
        public void MenuButtonHandler(GameObject buttonGameObject)
        {
            Debug.Log($"Clicked menu button: '{buttonGameObject.name}'");
            if (PlayerDataUtils.IsSoundEnabled())
            {
                GetComponent<AudioSource>().PlayOneShot(buttonClickSoundEffect);
            }

            switch (buttonGameObject.name)
            {
                // Main menu buttons
                case "PlayGameButton":
                case "BackToGameModeSelectButton":
                    ChangeMenuState(MainMenuState.GameModeSelect);
                    break;
                case "SettingsButton":
                    ChangeMenuState(MainMenuState.Settings);
                    break;
                case "ExitGameButton":
                    ExitGame();
                    break;
                
                // Settings menu buttons
                case "ToggleFullScreenButton":
                    ToggleFullScreen();
                    break;
                case "ToggleSoundButton":
                    ToggleSound();
                    break;
                case "BackToMainMenuButton":
                    ChangeMenuState(MainMenuState.Main);
                    break;
                
                // Game mode menu buttons
                case "ClassicTDModeButton":
                    SelectGameMode(GameUtils.scene_classicTD);
                    break;
                
                // Game map menu buttons
                case "VillageMapButton":
                    StartGame(villageMapPrefab);
                    break;
            }
        }
    }
}
