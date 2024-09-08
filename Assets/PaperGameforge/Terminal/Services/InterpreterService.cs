using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    public abstract class InterpreterService : ScriptableObject, ITerminalService
    {
        public abstract List<ServiceResponse> Execute(string userInput);
        public abstract List<ServiceResponse> Execute(List<string> userInput);
    }
}