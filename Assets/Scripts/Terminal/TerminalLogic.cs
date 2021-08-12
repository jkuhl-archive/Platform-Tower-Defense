using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terminal.Commands;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Terminal
{
    public class TerminalLogic : MonoBehaviour
    {
        // Terminal sizing variables
        // TODO: This should be updated based on resolution so we can fit more lines when we have room for them
        private const int MaxOutputLineCount = 29;
        private const int MaxOutputLineLength = 100;

        [Header("Terminal GameObjects")]
        [SerializeField] private GameObject terminalCanvas;
        [SerializeField] private GameObject terminalInputField;
        [SerializeField] private GameObject terminalOutputParent;
        [SerializeField] private GameObject terminalOutputLinePrefab;

        // Terminal usage variables
        private readonly List<ITerminalCommand> availableCommands = new List<ITerminalCommand>();
        private readonly List<GameObject> terminalOutputLines = new List<GameObject>();
        private readonly List<string> inputHistory = new List<string>();
        private bool isFading;
        private int historyIndex = -1;
        private string inputBuffer;

        // Start is called before the first frame update
        private void Start()
        {
            // Register the GameLoadingObject in GameUtils so we always have access to it,
            // destroy it if another is already registered
            var success = GameUtils.SetTerminalObject(gameObject);
            if (!success) Destroy(gameObject);

            // Populate list of available commands
            availableCommands.Add(new Clear());
            availableCommands.Add(new Exit());
            availableCommands.Add(new GiveBuff());
            availableCommands.Add(new GiveHealth());
            availableCommands.Add(new GiveMoney());
            availableCommands.Add(new Help());
            availableCommands.Add(new ListCreep());
            availableCommands.Add(new ListTower());
        }

        // Update is called once per frame
        private void Update()
        {
            // Check if we should toggle the terminal
            if (!GameUtils.GetGameLoadingLogic().IsGameLoading() && !isFading)
                if (Input.GetKeyUp(KeyCode.BackQuote))
                    ToggleTerminalView();

            // Auto select the terminal input field when we have the terminal open
            if (terminalCanvas.activeSelf && !isFading)
            {
                terminalInputField.GetComponent<InputField>().ActivateInputField();
                if (Input.GetKeyUp(KeyCode.UpArrow)) HistoryScrollUp();
                if (Input.GetKeyUp(KeyCode.DownArrow)) HistoryScrollDown();
            }
        }
        
        /// <summary>
        ///     Fades the terminal overlay in and out
        /// </summary>
        /// <param name="enable"> True to fade the terminal in, false if to fade it out </param>
        /// <returns> null while still in progress </returns>
        private IEnumerator FadeTerminal(bool enable)
        {
            isFading = true;
            var startValue = terminalCanvas.GetComponent<CanvasGroup>().alpha;
            float fadeLength = .2f;
            float targetValue = 0;
            float time = 0;

            if (enable)
            {
                terminalCanvas.SetActive(true);
                targetValue = .75f;
            }

            while (time < fadeLength)
            {
                terminalCanvas.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startValue, targetValue, time / fadeLength);
                time += Time.deltaTime;
                yield return null;
            }
            terminalCanvas.GetComponent<CanvasGroup>().alpha = targetValue;

            if (!enable) terminalCanvas.SetActive(false);
            
            isFading = false;
        }
        
        /// <summary>
        ///     Scrolls down through the terminal history
        /// </summary>
        private void HistoryScrollDown()
        {
            if (historyIndex != -1)
            {
                if (historyIndex < inputHistory.Count - 1)
                {
                    historyIndex++;
                    terminalInputField.GetComponent<InputField>().text = inputHistory[historyIndex];
                }
                else if (historyIndex >= inputHistory.Count - 1)
                {
                    terminalInputField.GetComponent<InputField>().text = inputBuffer;
                    inputBuffer = string.Empty;
                    historyIndex = -1;
                }

                var command = terminalInputField.GetComponent<InputField>().text;
                terminalInputField.GetComponent<InputField>().caretPosition = command.Length;
            }
        }

        /// <summary>
        ///     Scrolls up through the terminal history
        /// </summary>
        private void HistoryScrollUp()
        {
            if (inputHistory.Count > 0)
            {
                if (historyIndex == -1)
                {
                    inputBuffer = terminalInputField.GetComponent<InputField>().text;
                    historyIndex = inputHistory.Count;
                }

                if (historyIndex > 0) historyIndex--;

                var command = inputHistory[historyIndex];
                terminalInputField.GetComponent<InputField>().text = command;
                terminalInputField.GetComponent<InputField>().caretPosition = command.Length;
            }
        }

        /// <summary>
        ///     Returns the list of available terminal commands
        /// </summary>
        /// <returns> List of ITerminalCommand objects, one for each command </returns>
        public List<ITerminalCommand> GetAvailableCommands()
        {
            return availableCommands;
        }

        /// <summary>
        ///     Attempts to locate and execute a command based on the given terminal input
        /// </summary>
        /// <param name="terminalInput"> Input field that the player typed text in to </param>
        public void ProcessCommand(InputField terminalInput)
        {
            // Store input text and then clear the input field
            var commandString = terminalInput.text.ToLower();
            terminalInput.text = string.Empty;

            // If the given command string is empty return immediately
            if (commandString.Length <= 0 || commandString.Contains("`")) return;

            // Add command string to command history
            inputHistory.Add(commandString);
            historyIndex = -1;
            WriteToTerminalOutput(TerminalOutputType.InputRecord, commandString);

            // Split command string up into the base command and args
            var commandArgs = commandString.Split(' ').ToList();
            var commandName = commandArgs[0];
            commandArgs.Remove(commandName);

            // Locate the command we are trying to run
            ITerminalCommand selectedCommand = null;
            foreach (var command in availableCommands)
                if (command.GetCommandAliases().Contains(commandName))
                {
                    selectedCommand = command;
                    break;
                }

            // Attempt to execute the command;
            if (selectedCommand != null)
            {
                Debug.Log($"Executing terminal command: '{commandString}'");
                selectedCommand.Execute(commandArgs);
            }
            else
            {
                WriteToTerminalOutput(TerminalOutputType.Error, $"Command '{commandName}' does not exist");
            }
        }

        /// <summary>
        ///     Toggles the terminal canvas, enabling or disabling usage of the terminal
        /// </summary>
        public void ToggleTerminalView()
        {
            var toggle = !terminalCanvas.activeSelf;
            StartCoroutine(FadeTerminal(toggle));
        }

        /// <summary>
        ///     Removes lines from the output until we meet the given number of lines
        /// </summary>
        /// <param name="numberOfLines"> Number of lines that should be left in the output </param>
        public void TrimOutputLines(int numberOfLines)
        {
            while (terminalOutputLines.Count > numberOfLines)
            {
                var o = terminalOutputLines[0];
                terminalOutputLines.Remove(o);
                Destroy(o);
            }
        }

        /// <summary>
        ///     Writes text to the terminal output
        /// </summary>
        /// <param name="outputType"> Type of message we want to write to the output </param>
        /// <param name="outputString"> String that we want to write to the output </param>
        public void WriteToTerminalOutput(TerminalOutputType outputType, string outputString)
        {
            // Truncate strings that are longer that the max line length
            // TODO: Make this generate multiple lines instead of truncating
            if (outputString.Length > MaxOutputLineLength)
                outputString = $"{outputString.Substring(0, MaxOutputLineLength)} [...]";

            // Format output message based on output type
            var fontStyle = FontStyle.Normal;
            var textColor = Color.grey;
            switch (outputType)
            {
                case TerminalOutputType.Error:
                    outputString = $"ERROR: {outputString}";
                    textColor = Color.red;
                    break;
                case TerminalOutputType.Header:
                    fontStyle = FontStyle.BoldAndItalic;
                    break;
                case TerminalOutputType.Info:
                    break;
                case TerminalOutputType.InputRecord:
                    outputString = $"> {outputString}";
                    textColor = Color.green;
                    break;
            }

            // Instantiate output line object, populate it, and then add it to the terminal output
            var output = Instantiate(terminalOutputLinePrefab, Vector3.zero,
                terminalOutputLinePrefab.transform.rotation);
            output.GetComponent<Text>().text = outputString;
            output.GetComponent<Text>().fontStyle = fontStyle;
            output.GetComponent<Text>().color = textColor;
            output.transform.SetParent(terminalOutputParent.transform);
            output.transform.localScale = Vector3.one;
            terminalOutputLines.Add(output);

            // Trim output lines to fit in output box
            TrimOutputLines(MaxOutputLineCount);
        }
    }
}