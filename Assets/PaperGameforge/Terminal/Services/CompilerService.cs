using Assets.PaperGameforge.Terminal.Services.Responses;
using Assets.PaperGameforge.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    [CreateAssetMenu(fileName = "CompilerService", menuName = "TerminalServices/InterpreterServices/CompilerService")]
    public class CompilerService : InterpreterService
    {
        private readonly ErrorKey errorCompilationCmd = new("ERROR COMPILATION SCRIPT");
        private const string RUN_COMMAND = "run";
        private const char WHITE_SAPACE_SEPARATOR = ' ';
        private const int ERROR_PRIORITY = 8;

        public override List<ServiceResponse> Execute(string userInput)
        {
            string[] fileArgs = userInput.Split(WHITE_SAPACE_SEPARATOR);

            if (fileArgs.Length == 2)
            {
                if (fileArgs[0] == RUN_COMMAND)
                {
                    var filePath = fileArgs[1];

                    bool fileExists = File.Exists(filePath); // Using 'File' instead of 'Directory' for obvious reasons

                    if (fileExists)
                    {
                        var result = RuntimeCompiler.CompileAndRunFromFile(filePath);

                        string readableResult = TurnValueReadable(result);

                        return new() { new(readableResult, false) }; // Do not launch error
                    }

                    return new() { new ServiceError(errorCompilationCmd.Cmd, false, ERROR_PRIORITY) }; // Launch error
                }
            }

            return null;
        }
        public override List<ServiceResponse> Execute(List<string> userInput)
        {
            List<ServiceResponse> results = new();

            foreach (string dir in userInput)
            {
                results.AddRange(Execute(dir));
            }

            return results;
        }
        public string TurnValueReadable(System.Object value)
        {
            if (double.TryParse(Convert.ToString(value), out double _))
            {
                return Convert.ToString(value);
            }
            else
            {
                return value.ToString();
            }
        }
    }
}