using Assets.PaperGameforge.Terminal.Services.Interfaces;
using Assets.PaperGameforge.Terminal.Services.Responses;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    public abstract class InterpreterService : ScriptableObject, ITerminalService
    {
        public abstract List<ServiceResponse> Execute(string userInput);
        public abstract List<ServiceResponse> Execute(List<string> userInput);
    }
}