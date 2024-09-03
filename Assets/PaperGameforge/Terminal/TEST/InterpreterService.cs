﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    public abstract class InterpreterService : ScriptableObject, ITerminalService
    {
        public abstract (bool, List<string>) Execute(string userInput);
    }
}