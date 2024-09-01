using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ProcessedLine : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private string directory = string.Empty;
        [SerializeField] private string userInput = string.Empty;
        private TextMeshProUGUI text;
        #endregion

        #region PROPERTIES
        public TextMeshProUGUI Text
        {
            get
            {
                if (text == null)
                {
                    text = GetComponent<TextMeshProUGUI>();
                }
                return text;
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ProcessedLine(string directory, string userInput)
        {
            this.directory = directory;
            this.userInput = userInput;
            CreateTextInfo();
        }
        #endregion

        #region METHODS
        private void OnEnable()
        {
            CreateTextInfo();
        }
        public void CreateTextInfo(string directory = null, string userInput = null)
        {
            if (directory != null)
            {
                this.directory = directory;
            }
            if (userInput != null)
            {
                this.userInput = userInput;
            }

            Text.text = this.directory + this.userInput;
        }
        #endregion
    }
}