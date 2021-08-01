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
            availableCommands.Add(new GiveHealth());
            availableCommands.Add(new GiveMoney());
            availableCommands.Add(new Help());
        }

        // Update is called once per frame
        private void Update()
        {
            // Check if we should toggle the terminal
            if (!GameUtils.GetGameLoadingLogic().IsGameLoading())
                if (Input.GetKeyUp(KeyCode.BackQuote))
                    ToggleTerminalView();

            // Auto select the terminal input field when we have the terminal open
            if (terminalCanvas.activeSelf)
            {
                terminalInputField.GetComponent<InputField>().ActivateInputField();

                if (Input.GetKeyUp(KeyCode.UpArrow)) HistoryScrollUp();

                if (Input.GetKeyUp(KeyCode.DownArrow)) HistoryScrollDown();
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
        ///     Attempts to locate and execute a command based on the given terminal input
        /// </summary>
        /// <param name="terminalInput"> Input field that the player typed text in to </param>
        public void ProcessCommand(InputField terminalInput)
        {
            // Store input text and then clear the input field
            var commandString = terminalInput.text;
            terminalInput.text = string.Empty;

            // If the given command string is empty return immediately
            if (commandString.Length <= 0 || commandString == "`") return;

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
            terminalCanvas.SetActive(toggle);
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