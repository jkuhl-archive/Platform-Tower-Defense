using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Terminal
{
    public class TerminalLogic : MonoBehaviour
    {
        [Header("Terminal GameObjects")]
        [SerializeField] private GameObject terminalInput;
        [SerializeField] private GameObject terminalHistoryContent;
        [SerializeField] private GameObject terminalHistoryEntryPrefab;

        // Start is called before the first frame update
        void Start()
        {
            // Prevent GameLoadingObject from being destroyed when a new scene is loaded
            DontDestroyOnLoad(gameObject);   
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ProcessCommand(InputField commandInput)
        {
            string commandString = commandInput.text;
            commandInput.text = String.Empty;

            Debug.Log(commandString);
            
            GameObject newHistoryEntry = Instantiate(terminalHistoryEntryPrefab, Vector3.zero, 
                terminalHistoryEntryPrefab.transform.rotation);

            newHistoryEntry.GetComponent<Text>().text = $"> {commandString}";
            
            newHistoryEntry.transform.SetParent(terminalHistoryContent.transform); 
        }
    }
}
