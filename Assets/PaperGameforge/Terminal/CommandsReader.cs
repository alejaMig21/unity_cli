using Assets.PaperGameforge.Terminal.Commands;
using Assets.PaperGameforge.Utils;
using Assets.PaperGameforge.Utils.GenericTree;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    public static class CommandsReader
    {
        private const string FILE_NAME = "terminal_commands.csv";
        private const string NOT_FOUND_COMMAND = "ERROR NOT_FOUND";
        private static string[,] commands = null;
        private static List<CommandTree> commandsTree = null;

        public static string[,] Commands
        {
            get
            {
                if (commands == null)
                {
                    var csvFilePath = Path.Combine(Application.streamingAssetsPath, FILE_NAME);
                    commands = CsvFileReader.ReadCsvByColumns(csvFilePath);
                }
                return commands;
            }
        }
        public static List<CommandTree> CommandsTrees
        {
            get
            {
                if (commandsTree == null)
                {
                    var matrix = Commands;

                    commandsTree = BuildTrees();
                }
                return commandsTree;
            }
        }
        /// <summary>
        /// Builds a list of command trees from a predefined command matrix.
        /// </summary>
        /// <returns>A list of <see cref="CommandTree"/> objects representing the command trees.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the command matrix is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the command matrix is improperly formatted.</exception>
        public static List<CommandTree> BuildTrees()
        {
            // Initialize a list to hold the command trees and a dictionary to track nodes
            List<CommandTree> trees = new();
            Dictionary<string, ArgNode> nodeDict = new();

            // Get the number of rows and columns in the Commands array
            int rows = Commands.GetLength(0);
            int cols = Commands.GetLength(1);

            // Iterate through each column from the last to the first
            for (int col = cols - 1; col >= 0; col--)
            {
                // Iterate through each row
                for (int row = 0; row < rows; row++)
                {
                    string value = Commands[row, col];
                    // Skip empty values
                    if (!string.IsNullOrEmpty(value))
                    {
                        ArgNode currentNode;
                        // Create an Answer node for leaves (last column)
                        if (col == cols - 1)
                        {
                            currentNode = new Answer(value);
                        }
                        else
                        {
                            // If the node does not exist, create a new ArgNode and add it to the dictionary
                            if (!nodeDict.ContainsKey(value))
                            {
                                nodeDict[value] = new ArgNode(value);
                            }
                            currentNode = nodeDict[value];
                        }

                        // If it's the first column, create a new CommandTree and add it to the list
                        if (col == 0)
                        {
                            CommandTree tree = new(value);
                            tree.Root = currentNode;
                            trees.Add(tree);
                        }
                        else
                        {
                            bool parentFound = false;
                            // Search for a parent node in the previous columns of the same row
                            for (int parentCol = col - 1; parentCol >= 0; parentCol--)
                            {
                                string parentValue = Commands[row, parentCol];
                                if (!string.IsNullOrEmpty(parentValue))
                                {
                                    if (!nodeDict.ContainsKey(parentValue))
                                    {
                                        nodeDict[parentValue] = new ArgNode(parentValue);
                                    }

                                    ArgNode parentNode = nodeDict[parentValue];
                                    parentNode.AddChild(currentNode);
                                    parentFound = true;
                                    break;
                                }
                            }

                            // If no parent node is found in the same row, search in the previous rows
                            if (!parentFound)
                            {
                                for (int prevRow = row - 1; prevRow >= 0; prevRow--)
                                {
                                    for (int parentCol = col - 1; parentCol >= 0; parentCol--)
                                    {
                                        string parentValue = Commands[prevRow, parentCol];
                                        if (!string.IsNullOrEmpty(parentValue))
                                        {
                                            if (!nodeDict.ContainsKey(parentValue))
                                            {
                                                nodeDict[parentValue] = new ArgNode(parentValue);
                                            }

                                            ArgNode parentNode = nodeDict[parentValue];
                                            parentNode.AddChild(currentNode);
                                            parentFound = true;
                                            break;
                                        }
                                    }
                                    if (parentFound) { break; }
                                }
                            }
                        }

                        // Add the current node to the dictionary if it's not already present
                        if (!nodeDict.ContainsKey(value))
                        {
                            nodeDict[value] = currentNode;
                        }
                    }
                }
            }

            // Return the list of command trees
            return trees;
        }
        public static void PrintCommandTrees()
        {
            PrintCommandTrees(BuildTrees());
        }
        public static void PrintCommandTrees(List<CommandTree> commandTrees)
        {
            foreach (var tree in commandTrees)
            {
                Debug.Log($"Root: {tree.Root.Value}");
                PrintNode(tree.Root, "  ");
            }
        }
        private static void PrintNode(TreeNode<string> node, string indent)
        {
            foreach (var child in node.Children)
            {
                Debug.Log($"{indent}Child: {child.Value}");
                PrintNode(child, indent + "  ");
            }
        }
        /// <summary>
        /// Processes a command string and retrieves corresponding responses from a predefined command tree structure.
        /// </summary>
        /// <param name="command">The command string to be processed.</param>
        /// <returns>A tuple containing a boolean indicating if an error occurred and a list of response strings.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the command string is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the command string is empty or invalid.</exception>
        public static (bool error, List<string> responses) GetResponses(string command)
        {
            string[] args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> responses = new();

            //Traverse the tree to find the node corresponding to the command
            ArgNode currentNode = null;
            foreach (var tree in CommandsTrees)
            {
                if (tree.Root.Value == args[0])
                {
                    currentNode = tree.Root as ArgNode;
                    break;
                }
            }

            if (currentNode != null)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    var nextNode = currentNode.Children.Find(n => n.Value == args[i]) as ArgNode;
                    if (nextNode == null)
                    {
                        responses.AddRange(GetResponses(NOT_FOUND_COMMAND).responses);
                        return (true, responses);
                    }
                    currentNode = nextNode;
                }

                // Collect all Answer nodes from the current node
                CollectAnswerChildren(currentNode, responses);
            }

            if (responses.Count > 0)
            {
                return (false, responses);
            }

            responses.AddRange(GetResponses(NOT_FOUND_COMMAND).responses);
            return (true, responses);
        }
        private static void CollectAnswerChildren(ArgNode node, List<string> responses)
        {
            foreach (ArgNode child in node.Children)
            {
                if (child is Answer answerChild)
                {
                    responses.Add(answerChild.Value);
                }
            }
        }
    }
}