using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Assets.PaperGameforge.Utils
{
    public static class RuntimeCompiler
    {
        private const string METHOD_NAME = "Run";

        //private void Test()
        //{
        //    // TEST
        //    string filePath = "C:\\Users\\aleja\\Downloads\\NewTest.cs";

        //    Debug.Log((string)CompileAndRunFromFile(filePath));
        //}

        /// <summary>
        /// Compiles and runs the provided C# code from a file.
        /// </summary>
        /// <param name="filePath">The path to the file containing the C# code to compile and run.</param>
        /// <returns>The result of the compiled code execution, or null if compilation fails or the file does not exist.</returns>
        public static object CompileAndRunFromFile(string filePath)
        {
            // Check if the file exists at the provided path
            if (!File.Exists(filePath))
            {
                // Log an error if the file does not exist
                Debug.LogError("File not found: " + filePath);
                return null;
            }

            // Read the C# code from the file
            string code = File.ReadAllText(filePath);

            // Compile and run the C# code
            return CompileAndRun(code);
        }
        /// <summary>
        /// Compiles and runs the provided C# code from a TextAsset.
        /// </summary>
        /// <param name="textAsset">The TextAsset containing the C# code to compile and run.</param>
        /// <returns>The result of the compiled code execution, or null if compilation fails.</returns>
        public static object CompileAndRunFromTextAsset(TextAsset textAsset)
        {
            // Check if the provided TextAsset is null
            if (textAsset == null)
            {
                // Log an error if the TextAsset is null
                Debug.LogError("TextAsset is null");
                return null;
            }

            // Get the C# code from the TextAsset
            string code = textAsset.text;

            // Compile and run the C# code
            return CompileAndRun(code);
        }
        /// /// <summary>
        /// Compiles and runs the provided C# code.
        /// </summary>
        /// <param name="code">The C# code to compile and run.</param>
        /// <returns>The result of the compiled code execution, or null if compilation fails.</returns>
        public static object CompileAndRun(string code)
        {
            // Parse the provided code into a syntax tree
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Get references to all non-dynamic assemblies in the current domain
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            // Create a new C# compilation with the provided code and references
            var compilation = CSharpCompilation.Create("UserCode")
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // Emit the compiled code into a memory stream
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                // Check if compilation was successful
                if (result.Success)
                {
                    // Load the compiled assembly from the memory stream
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());

                    // Find the type with a 'Run' method
                    var type = assembly.GetTypes().FirstOrDefault(t => t.GetMethod(METHOD_NAME) != null);

                    // If no type with a 'Run' method is found, log an error and return null
                    if (type == null)
                    {
                        Debug.LogError($"No class with a {METHOD_NAME} method found.");
                        return null;
                    }

                    // Get the 'Run' method
                    var method = type.GetMethod(METHOD_NAME);

                    // Invoke the 'Run' method, either statically or on an instance
                    if (method.IsStatic)
                    {
                        return method.Invoke(null, null);
                    }
                    else
                    {
                        var instance = Activator.CreateInstance(type);
                        return method.Invoke(instance, null);
                    }
                }
                else
                {
                    // If compilation fails, log all diagnostics and return null
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Debug.LogError(diagnostic.ToString());
                    }
                    return null;
                }
            }
        }
    }
}