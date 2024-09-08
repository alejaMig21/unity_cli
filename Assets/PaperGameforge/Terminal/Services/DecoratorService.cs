using Assets.PaperGameforge.Terminal.TEST;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorService : ScriptableObject, ITerminalService
{
    protected const string D_METHOD_CONST = "<DMETHOD>";
    protected const char TWO_DOTS_SEPARATOR = ':';

    public virtual List<ServiceResponse> ParseResponse(List<ServiceResponse> responses)
    {
        List<ServiceResponse> finalList = new();

        foreach (var item in responses)
        {
            if (item.BackgroundProcess)
            {
                continue;
            }

            List<ServiceResponse> decoratedResponses = ProcessResponse(item);

            if (decoratedResponses != null)
            {
                // Añadir elementos decorados en lugar de la respuesta original
                finalList.AddRange(decoratedResponses);
            }
            else
            {
                // Añadir la respuesta original si no puede ser decorada
                finalList.Add(item);
            }
        }

        return finalList.Count > 0 ? finalList : null;
    }
    public virtual List<ServiceResponse> ProcessResponse(ServiceResponse response, string userInput = "")
    {
        string[] args = response.Text.Split(TWO_DOTS_SEPARATOR);

        if (args.Length > 1)
        {
            string commandType = args[0];
            string commandParam = args[1];

            if (commandType.Equals(D_METHOD_CONST))
            {
                object result = ExecuteMethod(commandParam);

                if (result is List<ServiceResponse> parsedResult && parsedResult.Count > 0)
                {
                    return parsedResult;
                }
            }
        }

        return null;
    }
    public virtual object ExecuteMethod(string methodName)
    {
        var method = GetType().GetMethod(methodName);

        if (method == null) { return null; }

        object result = method.Invoke(this, new object[0]);

        return result;
    }
}