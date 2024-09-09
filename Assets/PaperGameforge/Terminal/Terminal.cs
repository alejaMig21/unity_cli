using Assets.PaperGameforge.Terminal.Services;
using Assets.PaperGameforge.Terminal.Services.Responses;
using Assets.PaperGameforge.Terminal.UI.CustomTMP;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(Interpreter))]
    public class Terminal : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private GameObject processedLine;
        [SerializeField] private GameObject responseLine;
        [SerializeField] private TMP_InputField terminalInput;
        [SerializeField] private GameObject userInputLine;
        [SerializeField] private ScrollRect sr;
        [SerializeField] private GameObject msgList;
        private Interpreter interpreter;

        private List<ProcessedLine> pLines = new();
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
        public List<ProcessedLine> PLines { get => pLines; set => pLines = value; }
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
        private void Start()
        {
            //AddInterpreterLines(_Interpreter.Interpret("ascii"));
            // Move the user input line to the end.
            userInputLine.transform.SetAsLastSibling();
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
                int lines = AddInterpreterLines(_Interpreter.Interpret(userInput));

                // Scroll to the bottom of the scrollRect.
                ScrollToBottom(lines);

                // Move the user input line to the end.
                userInputLine.transform.SetAsLastSibling();

                // Refocus the input field.
                terminalInput.ActivateInputField();
                terminalInput.Select();
            }
        }
        private void ClearInputField()
        {
            terminalInput.text = string.Empty;
        }
        private void AddDirectoryLine(string userInput)
        {
            // Resizing the command line container, so the scrollRect doesn't throw a fit.
            Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new(msgListSize.x, msgListSize.y + PIXELS_PER_LINE);

            // Instantiate the directory line.
            GameObject msg = Instantiate(processedLine, msgList.transform);

            // Set its child index
            msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

            var line = msg.GetComponentsInChildren<ProcessedLine>();
            foreach (ProcessedLine child in line)
            {
                child.CreateTextInfo(GetCurrentRenderedDirectory(), userInput);
                pLines.Add(child);
            }

            OnLineProcessed?.Invoke();
        }
        private int AddInterpreterLines(List<ServiceResponse> interpretation)
        {
            if (interpretation == null) { return 0; }

            for (int i = 0; i < interpretation.Count; i++)
            {
                // Instantiate the responses line.
                GameObject res = Instantiate(responseLine, msgList.transform);

                // Set it to the end of all the messages.
                res.transform.SetAsLastSibling();

                // Get the size of the message list, and resize.
                Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
                msgList.GetComponent<RectTransform>().sizeDelta = new(listSize.x, listSize.y + PIXELS_PER_LINE);

                // Set the dirText of this responses line to be whatever the interpreter string is.
                res.GetComponentInChildren<TextMeshProUGUI>().text = interpretation[i].Text;
            }

            return interpretation.Count;
        }
        private void ScrollToBottom(int lines)
        {
            if (lines > 4)
            {
                sr.velocity = new(0, 8000);
            }
            else
            {
                sr.verticalNormalizedPosition = 0; // The bottom of the scrollRect
            }
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
            // Returns the first DirectoryTextUGUI dirText value found on the UserInputLine
            return directories[0].text;
        }
        private void ClearTerminal()
        {
            if (msgList != null)
            {
                for (int i = 0; i < msgList.transform.childCount; i++)
                {
                    GameObject currentChild = msgList.transform.GetChild(i).gameObject;

                    if (currentChild == userInputLine)
                    {
                        continue;
                    }

                    Destroy(currentChild);
                }
            }
            pLines.Clear();
        }
        #endregion
    }
}