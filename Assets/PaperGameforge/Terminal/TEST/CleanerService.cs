using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CleanerService", menuName = "TerminalServices/DecoratorServices/CleanerService")]
    public class CleanerService : DecoratorService
    {
        #region EVENTS
        public event System.Action OnClearStart;
        #endregion

        public void Clear()
        {
            OnClearStart?.Invoke();
        }
    }
}