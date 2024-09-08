using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CloseService", menuName = "TerminalServices/DecoratorServices/CloseService")]
    public class CloseService : DecoratorService
    {
        public void CloseTerminal()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}