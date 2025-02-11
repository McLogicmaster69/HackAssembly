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
        private static int _ifCounter = 0;
        private static int _ifLabelCounter = 0;

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
            _ifCounter = 0;
            _ifLabelCounter = 0;
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

            if (elements.Length == 4  || elements.Length > 5)
            {
                error = ErrorType.TooManyArguements;
                return EmptyOutput;
            }

            string[] evaluation;

            if (elements.Length == 3)
            {
                evaluation = EvaluateExpression(elements[2], state, out ErrorType evalError);
                if (evalError != ErrorType.None)
                {
                    error = evalError;
                    return EmptyOutput;
                }
            }
            else
            {
                evaluation = EvaluateExpression(elements[2], elements[3], elements[4], state, out ErrorType evalError);
                if (evalError != ErrorType.None)
                {
                    error = evalError;
                    return EmptyOutput;
                }
            }

            List<string> output = new List<string>(evaluation);
            string memoryAddress = $"@{state.VariableMemoryAddress(elements[0])}";
            output.Add(memoryAddress);
            output.Add("M=D");
            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateExpression(string p1, string op, string p2, ABCompileState state, out ErrorType error)
        {
            List<string> output = new List<string>();

            string[] p1Output = EvaluateExpression(p1, state, out ErrorType p1Error);
            if (p1Error != ErrorType.None)
            {
                error = p1Error;
                return EmptyOutput;
            }
            output.AddRange(p1Output);
            output.Add("@R0");
            output.Add("M=D");

            string[] p2Output = EvaluateExpression(p2, state, out ErrorType p2Error);
            if (p2Error != ErrorType.None)
            {
                error = p2Error;
                return EmptyOutput;
            }
            output.AddRange(p2Output);

            switch (op)
            {
                case "+":
                    output.Add("@R0");
                    output.Add("D=M+D");
                    break;
                case "-":
                    output.Add("@R0");
                    output.Add("D=M-D");
                    break;
                default:
                    error = ErrorType.UnknownOperator;
                    return EmptyOutput;
            }

            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateExpression(string p, ABCompileState state, out ErrorType error)
        {
            error = ErrorType.None;
            List<string> output = new List<string>();

            if (p == "true")
            {
                output.Add("D=-1");
                return output.ToArray();
            }

            if (p == "false")
            {
                output.Add("D=0");
                return output.ToArray();
            }

            if (!int.TryParse(p, out int result))
            {
                if (!state.ContainsVariable(p))
                {
                    error = ErrorType.InvalidAssignment;
                    return EmptyOutput;
                }

                error = ErrorType.None;
                return new string[]
                {
                    $"@{state.VariableMemoryAddress(p)}",
                    "D=M"
                };
            }

            if (result > 32767 || result < -32768)
            {
                error = ErrorType.InvalidAssignment;
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

            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateBooleanExpression(string p, string op, ABCompileState state, out ErrorType error)
        {
            List<string> output = new List<string>();

            string[] pLines = EvaluateExpression(p, state, out ErrorType pError);
            if (pError != ErrorType.None)
            {
                error = pError;
                return EmptyOutput;
            }
            output.AddRange(pLines);

            switch (op)
            {
                case "==":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JEZ";
                    break;
                case ">":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JGT";
                    break;
                case ">=":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JGE";
                    break;
                case "<":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JLT";
                    break;
                case "<=":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JLE";
                    break;
                case "!=":
                    output[output.Count - 1] = $"{output[output.Count - 1]};JNE";
                    break;
                default:
                    error = ErrorType.UnknownOperator;
                    return EmptyOutput;
            }

            error = ErrorType.None;
            return output.ToArray();
        }
        
        private static string[] Declare(string[] elements, ABCompileState state, out ErrorType error)
        {
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

        private static string[] StartIf(string[] elements, ABCompileState state, out ErrorType error)
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

            error = ErrorType.None;
            return EmptyOutput;
        }

        private static string[] GenerateError(int line, ErrorType type) => new string[] { $"ERROR ON LINE {line} OF TYPE {type}" };
    }

    public delegate string[] KeywordCommand(string[] elements, ABCompileState state, out ErrorType error);
}
