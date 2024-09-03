using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    public abstract class ResponseService : ScriptableObject, ITerminalService
    {
        public abstract List<string> ParseResponse(List<string> responses);
    }
}