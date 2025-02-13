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
        private static int _jumpCounter = 0;
        private static Stack<(int, StatementType)> _jumpToEnd;

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
            _keywords.Add("dec", DeclareStatement);
            _keywords.Add("if", IfStatement);
            _keywords.Add("end", EndStatement);
            _keywords.Add("ins", InsertStatement);
            _keywords.Add("rem", RemoveStatement);
            _keywords.Add("while", WhileStatement);
        }

        private static void InitialiseCompile()
        {
            if (!_initialised) Initialise();
            _jumpCounter = 0;
            _jumpToEnd = new Stack<(int, StatementType)>();
        }

        public static string[] Compile(string[] input)
        {
            InitialiseCompile();
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

            if (_jumpToEnd.Count > 0) return GenerateError(lineNumber, ErrorType.MissingEndStatement);

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

        private static string[] EvaluateBooleanExpression(string p, string op, string jumpLabel, ABCompileState state, out ErrorType error)
        {
            List<string> output = new List<string>();

            string[] pLines = EvaluateExpression(p, state, out ErrorType pError);
            if (pError != ErrorType.None)
            {
                error = pError;
                return EmptyOutput;
            }
            output.AddRange(pLines);
            output.Add($"@{jumpLabel}");

            switch (op)
            {
                case "==":
                    output.Add("D;JNE");
                    break;
                case ">":
                    output.Add("D;JLE");
                    break;
                case ">=":
                    output.Add("D;JLT");
                    break;
                case "<":
                    output.Add("D;JGE");
                    break;
                case "<=":
                    output.Add("D;JGT");
                    break;
                case "!=":
                    output.Add("D;JEQ");
                    break;
                default:
                    error = ErrorType.UnknownOperator;
                    return EmptyOutput;
            }

            error = ErrorType.None;
            return output.ToArray();
        }
        
        private static string[] DeclareStatement(string[] elements, ABCompileState state, out ErrorType error)
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

            error = state.Declare(elements[1], out bool requireInit);
            if (!requireInit) return EmptyOutput;
            string[] assignment = VariableAssignment(new string[] { elements[1], "=", "0" }, state, out ErrorType assignmentError);
            if (assignmentError != ErrorType.None)
            {
                error = assignmentError;
                return EmptyOutput;
            }
            return assignment;
        }

        private static string[] IfStatement(string[] elements, ABCompileState state, out ErrorType error)
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

            string[] ifLines = EvaluateBooleanExpression(elements[1], elements[2], $"jump{_jumpCounter}", state, out ErrorType booleanError);
            _jumpToEnd.Push((_jumpCounter, StatementType.If));
            _jumpCounter++;
            if (booleanError != ErrorType.None)
            {
                error = booleanError;
                return EmptyOutput;
            }

            error = ErrorType.None;
            return ifLines;
        }

        private static string[] EndStatement(string[] elements, ABCompileState state, out ErrorType error)
        {
            if (elements.Length > 1)
            {
                error = ErrorType.TooManyArguements;
                return EmptyOutput;
            }

            if (_jumpToEnd.Count == 0)
            {
                error = ErrorType.NoCorrespondingStatement;
                return EmptyOutput;
            }

            (int, StatementType) jump = _jumpToEnd.Pop();
            switch (jump.Item2)
            {
                case StatementType.If:
                    error = ErrorType.None;
                    return new string[1] { $"(jump{jump.Item1})" };
                case StatementType.While:
                    error = ErrorType.None;
                    return new string[3] { $"@while{jump.Item1}", "0;JMP", $"(jump{jump.Item1})" };
            }

            error = ErrorType.UnknownCommand;
            return EmptyOutput;
        }

        private static string[] InsertStatement(string[] elements, ABCompileState state, out ErrorType error)
        {
            error = ErrorType.None;
            string output = "";
            for (int i = 1; i < elements.Length; i++)
            {
                output += elements[i];
            }
            return new string[1] { output };
        }

        private static string[] RemoveStatement(string[] elements, ABCompileState state, out ErrorType error)
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

            error = state.Remove(elements[1]);
            return EmptyOutput;
        }

        private static string[] WhileStatement(string[] elements, ABCompileState state, out ErrorType error)
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

            List<string> output = new List<string>();
            output.Add($"(while{_jumpCounter})");
            string[] whileLines = EvaluateBooleanExpression(elements[1], elements[2], $"jump{_jumpCounter}", state, out ErrorType booleanError);
            _jumpToEnd.Push((_jumpCounter, StatementType.While));
            _jumpCounter++;

            if (booleanError != ErrorType.None)
            {
                error = booleanError;
                return EmptyOutput;
            }

            output.AddRange(whileLines);
            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] ScreenStatement(string[] elements, ABCompileState state, out ErrorType error)
        {
            // 32 by 256

            if (elements.Length < 4)
            {
                error = ErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 4)
            {
                error = ErrorType.TooManyArguements;
                return EmptyOutput;
            }

            if (!int.TryParse(elements[1], out int x))
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (!int.TryParse(elements[2], out int y))
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (x < 0 || x > 31)
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (y < 0 || y > 255)
            {
                error = ErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            string[] valueLines = EvaluateExpression(elements[3], state, out ErrorType valueError);
            if (valueError != ErrorType.None)
            {
                error = valueError;
                return EmptyOutput;
            }

            List<string> output = new List<string>(valueLines);
            output.Add($"@{16384 + x + 32 * y}");
            output.Add("M=D");
            error = ErrorType.None;
            return output.ToArray();
        }

        private static string[] GenerateError(int line, ErrorType type) => new string[] { $"ERROR ON LINE {line} OF TYPE {type}" };
    }

    public delegate string[] KeywordCommand(string[] elements, ABCompileState state, out ErrorType error);
}
