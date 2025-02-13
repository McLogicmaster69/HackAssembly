using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.VM
{
    public static class VirtualMachineCompiler
    {
        private static bool _initialised = false;
        private static Dictionary<string, ABKeywordCommand> _keywords;

        private static string[] EmptyOutput => new string[0];

        private static void Initialise()
        {
            if (_initialised) return;
            _initialised = true;
            InitKeywords();
        }
        private static void InitKeywords()
        {
            _keywords = new Dictionary<string, ABKeywordCommand>();
            _keywords.Add("push", PushCommand);
            _keywords.Add("pop", PopCommand);
        }

        public static string[] Compile(string[] input)
        {
            if (!_initialised) Initialise();
            List<string> output = new List<string>();
            int lineNumber = -1;
            output.Add("@256");
            output.Add("D=A");
            output.Add("@SP");
            output.Add("M=D");

            foreach (string command in input)
            {
                string workingCommand = command.Trim();
                lineNumber++;
                if (string.IsNullOrEmpty(workingCommand)) continue;

                string[] elements = RemoveBlankSpace(workingCommand.Split(' '));

                if (_keywords.ContainsKey(elements[0]))
                {
                    string[] lines = _keywords[elements[0]](elements, out VMErrorType error);
                    if (error != VMErrorType.None) return GenerateError(lineNumber, error);
                    output.AddRange(lines);
                    continue;
                }

                return GenerateError(lineNumber, VMErrorType.UnknownCommand);
            }

            return output.ToArray();
        }

        private static string[] PushCommand(string[] elements, out VMErrorType error)
        {
            if (elements.Length < 3)
            {
                error = VMErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 3)
            {
                error = VMErrorType.TooManyArguements;
                return EmptyOutput;
            }

            List<string> output = new List<string>();

            switch (elements[1])
            {
                case "constant":
                    output.AddRange(PushConstantCommand(elements, out VMErrorType pError));
                    if (pError != VMErrorType.None)
                    {
                        error = pError;
                        return EmptyOutput;
                    }
                    break;
                default:
                    error = VMErrorType.InvalidScope;
                    return EmptyOutput;
            }

            output.Add("@SP");
            output.Add("A=M");
            output.Add("M=D");
            output.Add("@SP");
            output.Add("M=M+1");
            error = VMErrorType.None;
            return output.ToArray();
        }

        private static string[] PushConstantCommand(string[] elements, out VMErrorType error)
        {
            List<string> output = new List<string>();

            if (!int.TryParse(elements[2], out int result))
            {
                error = VMErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (result > 32767 || result < -32768)
            {
                error = VMErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (result == -32768)
            {
                output.Add($"@{32767}");
                output.Add("D=A");
                output.Add("D=-D");
                output.Add("D=D-1");
            }

            else if (result < 0)
            {
                output.Add($"@{-result}");
                output.Add("D=-A");
            }

            else
            {
                output.Add($"@{result}");
                output.Add("D=A");
            }

            error = VMErrorType.None;
            return output.ToArray();
        }

        private static string[] PopCommand(string[] elements, out VMErrorType error)
        {
            if (elements.Length < 3)
            {
                error = VMErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 3)
            {
                error = VMErrorType.TooManyArguements;
                return EmptyOutput;
            }

            List<string> output = new List<string>();

            switch (elements[1])
            {
                case "local":
                    break;
                default:
                    error = VMErrorType.InvalidScope;
                    return EmptyOutput;
            }

            error = VMErrorType.None;
            return output.ToArray();
        }

        private static string[] PopLocalCommand(string[] elements, out VMErrorType error)
        {

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

        private static string[] GenerateError(int line, VMErrorType type) => new string[] { $"ERROR ON LINE {line} OF TYPE {type}" };
    }

    public delegate string[] ABKeywordCommand(string[] elements, out VMErrorType error);
}
