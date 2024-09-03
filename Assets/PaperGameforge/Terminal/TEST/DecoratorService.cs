using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    public abstract class DecoratorService : ScriptableObject, ITerminalService
    {
        protected const string D_METHOD_CONST = "DMETHOD";
        protected const char TWO_DOTS_SEPARATOR = ':';

        public virtual (bool, List<string>) ParseResponse(List<string> responses)
        {
            List<string> list = new();

            foreach (var item in responses)
            {
                (bool error, List<string> decoratedResponses) = ProcessResponse(item);

                if (!error)
                {
                    list.AddRange(decoratedResponses);
                }
                else
                {
                    list.Add(item);
                }
            }

            if (list != null && list.Count > 0)
            {
                return (false, list);
            }
            return (true, null);
        }
        public virtual (bool, List<string>) ProcessResponse(string response, string userInput = "")
        {
            string[] args = response.Split(TWO_DOTS_SEPARATOR);

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                if (commandType.Equals(D_METHOD_CONST))
                {
                    object result = ExecuteMethod(commandParam);

                    if (result != null && result is List<string> parsedResult)
                    {
                        return (false, parsedResult);
                    }
                }
            }

            return (true, null);
        }
        public virtual object ExecuteMethod(string methodName)
        {
            var method = GetType().GetMethod(methodName);

            if (method == null) { return null; }

            object result = method.Invoke(this, new object[0]);

            return result;
        }
    }
}