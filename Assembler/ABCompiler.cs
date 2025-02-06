using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public static class ABCompiler
    {
        private static bool _initialised = false;
        private static Dictionary<string, KeywordCommand> _keywords;

        private static string[] EmptyOutput => new string[0];

        private static void Initialise()
        {
            if (_initialised) return;
            _initialised = true;
            InitKeywords();
        }

        private static void InitKeywords()
        {
            _keywords = new Dictionary<string, KeywordCommand>();
            _keywords.Add("dec", Declare);
        }

        public static string[] Compile(string[] input)
        {
            if (!_initialised) Initialise();
            List<string> output = new List<string>();
            ABCompileState state = new ABCompileState();
            int lineNumber = -1;

            foreach (string command in input)
            {
                string workingCommand = command.Trim();
                lineNumber++;
                if (string.IsNullOrEmpty(workingCommand)) continue;

                string[] elements = RemoveBlankSpace(workingCommand.Split(' '));

                if (_keywords.ContainsKey(elements[0]))
                {
                    string[] lines = _keywords[elements[0]](elements, state, out ErrorType error);
                    if (error != ErrorType.None) return GenerateError(lineNumber, error);
                    output.AddRange(lines);
                    continue;
                }

                if (state.ContainsVariable(elements[0]))
                {
                    string[] lines = VariableCommand(elements, state, out ErrorType error);
                    if (error != ErrorType.None) return GenerateError(lineNumber, error);
                    output.AddRange(lines);
                    continue;
                }

                return GenerateError(lineNumber, ErrorType.UnknownCommand);
            }

            return output.ToArray();
        }

        private static string[] RemoveBlankSpace(string[] input)
        {
            List<string> output = new List<string>();
            foreach (string s in input)
            {
                if (!string.IsNullOrEmpty(s)) output.Add(s);
            }
            return output.ToArray();
        }

        private static string[] VariableCommand(string[] elements, ABCompileState state, out ErrorType error)
        {
            error = ErrorType.None;

            if (elements.Length == 1)
            {
                error = ErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements[1] == "=") return VariableAssignment(elements, state, out error);

            error = ErrorType.UnknownOperator;
            return EmptyOutput;
        }

        private static string[] VariableAssignment(string[] elements, ABCompileState state, out ErrorType error)
        {
            if (elements.Length < 3)
            {
                error = ErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 3)
            {
                error = ErrorType.TooManyArguements;
                return EmptyOutput;
            }

            if (!int.TryParse(elements[2], out int result))
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (result > 32767 || result < -32768)
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            List<string> output = new List<string>();
            string memoryAddress = $"@{state.VariableMemoryAddress(elements[0])}";

            if (result == -32768)
            {
                output.Add($"@{32767}");
                output.Add("D=A");
                output.Add(memoryAddress);
                output.Add("M=-D");
                output.Add("M=M-1");
            }

            else if (result < 0)
            {
                output.Add($"@{-result}");
                output.Add("D=A");
                output.Add(memoryAddress);
                output.Add("M=-D");
            }

            else
            {
                output.Add($"@{result}");
                output.Add("D=A");
                output.Add(memoryAddress);
                output.Add("M=D");
            }

            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] Declare(string[] elements, ABCompileState state, out ErrorType error)
        {
            error = ErrorType.None;

            if (elements.Length < 2)
            {
                error = ErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 2)
            {
                error = ErrorType.TooManyArguements;
                return EmptyOutput;
            }

            error = state.Declare(elements[1]);
            return EmptyOutput;
        }

        private static string[] GenerateError(int line, ErrorType type) => new string[] { $"ERROR ON LINE {line} OF TYPE {type}" };
    }

    public delegate string[] KeywordCommand(string[] elements, ABCompileState state, out ErrorType error);
}
