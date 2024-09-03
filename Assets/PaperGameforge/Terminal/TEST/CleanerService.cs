using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CleanerService", menuName = "TerminalServices/InterpreterServices/CleanerService")]
    public class CleanerService : InterpreterService
    {
        #region EVENTS
        public event Action OnClearStart;
        #endregion

        public override (bool, List<string>) Execute(string userInput)
        {
            throw new System.NotImplementedException();
        }
        public void Clear()
        {
            OnClearStart?.Invoke();
        }
    }
}