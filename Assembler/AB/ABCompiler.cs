using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.AB
{
    public static class ABCompiler
    {
        private static bool _initialised = false;
        private static Dictionary<string, ABKeywordCommand> _keywords;
        private static int _jumpCounter = 0;
        private static Stack<(int, ABStatementType)> _jumpToEnd;

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
            _keywords.Add("dec", DeclareStatement);
            _keywords.Add("if", IfStatement);
            _keywords.Add("end", EndStatement);
            _keywords.Add("ins", InsertStatement);
            _keywords.Add("rem", RemoveStatement);
            _keywords.Add("while", WhileStatement);
            _keywords.Add("scr", ScreenStatement);
        }

        private static void InitialiseCompile()
        {
            if (!_initialised) Initialise();
            _jumpCounter = 0;
            _jumpToEnd = new Stack<(int, ABStatementType)>();
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
                    string[] lines = _keywords[elements[0]](elements, state, out ABErrorType error);
                    if (error != ABErrorType.None) return GenerateError(lineNumber, error);
                    output.AddRange(lines);
                    continue;
                }

                if (state.ContainsVariable(elements[0]))
                {
                    string[] lines = VariableCommand(elements, state, out ABErrorType error);
                    if (error != ABErrorType.None) return GenerateError(lineNumber, error);
                    output.AddRange(lines);
                    continue;
                }

                return GenerateError(lineNumber, ABErrorType.UnknownCommand);
            }

            if (_jumpToEnd.Count > 0) return GenerateError(lineNumber, ABErrorType.MissingEndStatement);

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

        private static string[] VariableCommand(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length == 1)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements[1] == "=") return VariableAssignment(elements, state, out error);

            error = ABErrorType.UnknownOperator;
            return EmptyOutput;
        }

        private static string[] VariableAssignment(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length < 3)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length == 4  || elements.Length > 5)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            string[] evaluation;

            if (elements[2] == $"!{elements[0]}")
            {
                error = ABErrorType.None;
                return new string[]
                {
                    $"@{state.VariableMemoryAddress(elements[0])}",
                    "M=!M"
                };
            }
            else if (elements.Length == 3)
            {
                evaluation = EvaluateExpression(elements[2], state, out ABErrorType evalError);
                if (evalError != ABErrorType.None)
                {
                    error = evalError;
                    return EmptyOutput;
                }
            }
            else
            {
                evaluation = EvaluateExpression(elements[2], elements[3], elements[4], state, out ABErrorType evalError);
                if (evalError != ABErrorType.None)
                {
                    error = evalError;
                    return EmptyOutput;
                }
            }

            List<string> output = new List<string>(evaluation);
            string memoryAddress = $"@{state.VariableMemoryAddress(elements[0])}";
            output.Add(memoryAddress);
            output.Add("M=D");
            error = ABErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateExpression(string p1, string op, string p2, ABCompileState state, out ABErrorType error)
        {
            List<string> output = new List<string>();

            string[] p1Output = EvaluateExpression(p1, state, out ABErrorType p1Error);
            if (p1Error != ABErrorType.None)
            {
                error = p1Error;
                return EmptyOutput;
            }
            output.AddRange(p1Output);
            output.Add("@R0");
            output.Add("M=D");

            string[] p2Output = EvaluateExpression(p2, state, out ABErrorType p2Error);
            if (p2Error != ABErrorType.None)
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
                case "&":
                    output.Add("@R0");
                    output.Add("D=M&D");
                    break;
                case "|":
                    output.Add("@R0");
                    output.Add("D=M|D");
                    break;
                default:
                    error = ABErrorType.UnknownOperator;
                    return EmptyOutput;
            }

            error = ABErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateExpression(string p, ABCompileState state, out ABErrorType error)
        {
            error = ABErrorType.None;
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

            if (p == ABCompileState.KEYBOARD_NAME)
            {
                error = ABErrorType.None;
                return new string[]
                {
                    "@24576",
                    "D=M"
                };
            }

            if (!int.TryParse(p, out int result))
            {
                if(p[0] == '!')
                {
                    if (!state.ContainsVariable(p.Substring(1)))
                    {
                        error = ABErrorType.InvalidAssignment;
                        return EmptyOutput;
                    }

                    error = ABErrorType.None;
                    return new string[]
                    {
                    $"@{state.VariableMemoryAddress(p.Substring(1))}",
                    "D=!M"
                    };
                }
                else
                {
                    if (!state.ContainsVariable(p))
                    {
                        error = ABErrorType.InvalidAssignment;
                        return EmptyOutput;
                    }

                    error = ABErrorType.None;
                    return new string[]
                    {
                    $"@{state.VariableMemoryAddress(p)}",
                    "D=M"
                    };
                }
            }

            if (result > 32767 || result < -32768)
            {
                error = ABErrorType.InvalidAssignment;
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

            error = ABErrorType.None;
            return output.ToArray();
        }

        private static string[] EvaluateBooleanExpression(string p, string op, string jumpLabel, ABCompileState state, out ABErrorType error)
        {
            List<string> output = new List<string>();

            string[] pLines = EvaluateExpression(p, state, out ABErrorType pError);
            if (pError != ABErrorType.None)
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
                    error = ABErrorType.UnknownOperator;
                    return EmptyOutput;
            }

            error = ABErrorType.None;
            return output.ToArray();
        }
        
        private static string[] DeclareStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length < 2)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 2)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            error = state.Declare(elements[1], out bool requireInit);
            if (!requireInit) return EmptyOutput;
            string[] assignment = VariableAssignment(new string[] { elements[1], "=", "0" }, state, out ABErrorType assignmentError);
            if (assignmentError != ABErrorType.None)
            {
                error = assignmentError;
                return EmptyOutput;
            }
            return assignment;
        }

        private static string[] IfStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length < 3)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 3)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            string[] ifLines = EvaluateBooleanExpression(elements[1], elements[2], $"jump{_jumpCounter}", state, out ABErrorType booleanError);
            _jumpToEnd.Push((_jumpCounter, ABStatementType.If));
            _jumpCounter++;
            if (booleanError != ABErrorType.None)
            {
                error = booleanError;
                return EmptyOutput;
            }

            error = ABErrorType.None;
            return ifLines;
        }

        private static string[] EndStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length > 1)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            if (_jumpToEnd.Count == 0)
            {
                error = ABErrorType.NoCorrespondingStatement;
                return EmptyOutput;
            }

            (int, ABStatementType) jump = _jumpToEnd.Pop();
            switch (jump.Item2)
            {
                case ABStatementType.If:
                    error = ABErrorType.None;
                    return new string[1] { $"(jump{jump.Item1})" };
                case ABStatementType.While:
                    error = ABErrorType.None;
                    return new string[3] { $"@while{jump.Item1}", "0;JMP", $"(jump{jump.Item1})" };
            }

            error = ABErrorType.UnknownCommand;
            return EmptyOutput;
        }

        private static string[] InsertStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            error = ABErrorType.None;
            string output = "";
            for (int i = 1; i < elements.Length; i++)
            {
                output += elements[i];
            }
            return new string[1] { output };
        }

        private static string[] RemoveStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length < 2)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 2)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            error = state.Remove(elements[1]);
            return EmptyOutput;
        }

        private static string[] WhileStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            if (elements.Length < 3)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 3)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            List<string> output = new List<string>();
            output.Add($"(while{_jumpCounter})");
            string[] whileLines = EvaluateBooleanExpression(elements[1], elements[2], $"jump{_jumpCounter}", state, out ABErrorType booleanError);
            _jumpToEnd.Push((_jumpCounter, ABStatementType.While));
            _jumpCounter++;

            if (booleanError != ABErrorType.None)
            {
                error = booleanError;
                return EmptyOutput;
            }

            output.AddRange(whileLines);
            error = ABErrorType.None;
            return output.ToArray();
        }

        private static string[] ScreenStatement(string[] elements, ABCompileState state, out ABErrorType error)
        {
            // 32 by 256

            if (elements.Length < 4)
            {
                error = ABErrorType.MissingArguements;
                return EmptyOutput;
            }

            if (elements.Length > 4)
            {
                error = ABErrorType.TooManyArguements;
                return EmptyOutput;
            }

            if (!int.TryParse(elements[1], out int x))
            {
                error = ABErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (!int.TryParse(elements[2], out int y))
            {
                error = ABErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (x < 0 || x > 31)
            {
                error = ABErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            if (y < 0 || y > 255)
            {
                error = ABErrorType.InvalidAssignment;
                return EmptyOutput;
            }

            string[] valueLines = EvaluateExpression(elements[3], state, out ABErrorType valueError);
            if (valueError != ABErrorType.None)
            {
                error = valueError;
                return EmptyOutput;
            }

            List<string> output = new List<string>(valueLines);
            output.Add($"@{16384 + x + 32 * y}");
            output.Add("M=D");
            error = ABErrorType.None;
            return output.ToArray();
        }

        private static string[] GenerateError(int line, ABErrorType type) => new string[] { $"ERROR ON LINE {line} OF TYPE {type}" };
    }

    public delegate string[] ABKeywordCommand(string[] elements, ABCompileState state, out ABErrorType error);
}
