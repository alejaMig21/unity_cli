using Assets.PaperGameforge.Terminal.Services;
using Assets.PaperGameforge.Terminal.Services.Responses;
using Assets.PaperGameforge.Terminal.UI.CustomTMP;
using Assets.PaperGameforge.Terminal.UI.InfiniteScroller;
using Gpm.Ui;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(Interpreter))]
    public class Terminal : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private TMP_InputField terminalInput;
        [SerializeField] private GameObject userInputLine;
        [SerializeField] private InfiniteScroll scroller;
        private Interpreter interpreter;
        private List<ProcessedLineData> pLines = new();
        private List<ServiceResponse> responses = new();
        private bool blockedTerminal = false;
        #endregion

        #region CONSTANTS
        private const int PIXELS_PER_LINE = 35;
        #endregion

        #region PROPERTIES
        public Interpreter _Interpreter
        {
            get
            {
                if (interpreter == null)
                {
                    interpreter = GetComponent<Interpreter>();
                }
                return interpreter;
            }
        }
        public List<ProcessedLineData> PLines { get => pLines; set => pLines = value; }
        public List<ServiceResponse> Responses { get => responses; set => responses = value; }
        #endregion

        #region EVENTS
        public event Action OnLineProcessed;
        #endregion

        #region METHODS
        private void Awake()
        {
            var services = _Interpreter.GetServices();

            foreach (var service in services)
            {
                if (service is CleanerService)
                {
                    (service as CleanerService).OnClearStart += ClearTerminal;
                    break;
                }
            }
        }
        private void OnGUI()
        {
            if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
            {
                // Store user typing
                string userInput = terminalInput.text;

                // Clear the input field
                ClearInputField();

                // Instantiate a gameobject with a directory prefix
                AddDirectoryLine(userInput);

                // Add the interpretation lines.
                AddInterpreterLines(_Interpreter.Interpret(userInput));

                scroller.MoveToLastData();

                HandleCopies();

                ResetTerminalInput();
            }
        }
        /// <summary>
        /// Refocus the input field.
        /// </summary>
        private void ResetTerminalInput()
        {
            if (blockedTerminal)
            {
                terminalInput.interactable = false;
                return;
            }

            terminalInput.interactable = true;
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
        private void HandleCopies()
        {
            if (responses.Count > 0)
            {
                int count = responses.Count;
                ProgressResponse lastResponse =
                    responses[count - 1] != null && responses[count - 1] is ProgressResponse ?
                    responses[count - 1] as ProgressResponse :
                    null;

                if (lastResponse != null)
                {
                    lastResponse.OnProgressStarted += () => blockedTerminal = true;
                    lastResponse.OnProgressFinished += () =>
                    {
                        blockedTerminal = false;
                        ResetTerminalInput();
                    };
                    StartCoroutine(lastResponse.SimulateProgress(scroller));
                }
            }
        }
        private void ClearInputField()
        {
            terminalInput.text = string.Empty;
        }
        private void AddDirectoryLine(string userInput)
        {
            ProcessedLineData data = new(GetCurrentRenderedDirectory(), userInput);

            scroller.InsertData(data);

            pLines.Add(data);

            OnLineProcessed?.Invoke();
        }
        private int AddInterpreterLines(List<ServiceResponse> interpretation)
        {
            if (interpretation == null) { return 0; }

            for (int i = 0; i < interpretation.Count; i++)
            {
                if (!interpretation[i].BackgroundProcess)
                {
                    // Tracks all of the interpretations received
                    responses.Add(interpretation[i]);

                    scroller.InsertData(new TerminalData(interpretation[i].Text));
                }
            }

            return interpretation.Count;
        }
        private string GetCurrentRenderedDirectory()
        {
            var directories = userInputLine.GetComponentsInChildren<DirectoryTextUGUI>();

#if UNITY_EDITOR
            if (directories.Length > 1)
            {
                Debug.LogWarning("\'userInputLine\' has more than one children with \'DirectoryTextUGUI\' components.\nThe first one will only be returned out of simplicity!");
            }
#endif
            // Returns the first DirectoryTextUGUI dirText percentValue found on the UserInputLine
            return directories[0].text;
        }
        private void ClearTerminal()
        {
            scroller.Clear();

            responses.Clear();

            /// Line below uncommented to achieve os cli behavior.
            /// Even after clearing a console, previous commands should be reachable.
            /// Uncomment if you are not interested in this behavior.
            //pLines.Clear();
        }
        #endregion
    }
}