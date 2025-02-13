using System.Collections.Generic;
using System.Linq;

namespace Assembler.Assembler
{
    public static class MainAssembler
    {
        private static bool _initialised = false;
        private static Dictionary<string, string> _comp;
        private static Dictionary<string, string> _dest;
        private static Dictionary<string, string> _jump;

        private static void Initialise()
        {
            if (_initialised) return;
            _initialised = true;
            InitComp();
            InitDest();
            InitJump();
        }

        private static void InitComp()
        {
            _comp = new Dictionary<string, string>();
            _comp.Add("0", "0101010");
            _comp.Add("1", "0111111");
            _comp.Add("-1", "0111010");
            _comp.Add("D", "0001100");
            _comp.Add("A", "0110000");
            _comp.Add("M", "1110000");
            _comp.Add("!D", "0001101");
            _comp.Add("!A", "0110001");
            _comp.Add("!M", "1110001");
            _comp.Add("-D", "0001111");
            _comp.Add("-A", "0110011");
            _comp.Add("-M", "1110011");
            _comp.Add("D+1", "0011111");
            _comp.Add("1+D", "0011111");
            _comp.Add("A+1", "0110111");
            _comp.Add("1+A", "0110111");
            _comp.Add("M+1", "1110111");
            _comp.Add("1+M", "1110111");
            _comp.Add("D-1", "0001110");
            _comp.Add("A-1", "0110010");
            _comp.Add("M-1", "1110010");
            _comp.Add("D+A", "0000010");
            _comp.Add("A+D", "0000010");
            _comp.Add("D+M", "1000010");
            _comp.Add("M+D", "1000010");
            _comp.Add("D-A", "0010011");
            _comp.Add("D-M", "1010011");
            _comp.Add("A-D", "0000111");
            _comp.Add("M-D", "1000111");
            _comp.Add("D&A", "0000000");
            _comp.Add("D&M", "1000000");
            _comp.Add("D|A", "0010101");
            _comp.Add("D|M", "1010101");
        }

        private static void InitDest()
        {
            _dest = new Dictionary<string, string>();
            _dest.Add("", "000");
            _dest.Add("M", "001");
            _dest.Add("D", "010");
            _dest.Add("DM", "011");
            _dest.Add("A", "100");
            _dest.Add("AM", "101");
            _dest.Add("AD", "110");
            _dest.Add("ADM", "111");
        }

        private static void InitJump()
        {
            _jump = new Dictionary<string, string>();
            _jump.Add("", "000");
            _jump.Add("JGT", "001");
            _jump.Add("JEQ", "010");
            _jump.Add("JGE", "011");
            _jump.Add("JLT", "100");
            _jump.Add("JNE", "101");
            _jump.Add("JLE", "110");
            _jump.Add("JMP", "111");
        }

        private static Dictionary<string, int> InitLabels()
        {
            Dictionary<string, int> labels = new Dictionary<string, int>();
            labels.Add("R0", 0);
            labels.Add("R1", 1);
            labels.Add("R2", 2);
            labels.Add("R3", 3);
            labels.Add("R4", 4);
            labels.Add("R5", 5);
            labels.Add("R6", 6);
            labels.Add("R7", 7);
            labels.Add("R8", 8);
            labels.Add("R9", 9);
            labels.Add("R10", 10);
            labels.Add("R11", 11);
            labels.Add("R12", 12);
            labels.Add("R13", 13);
            labels.Add("R14", 14);
            labels.Add("R15", 15);
            labels.Add("SCREEN", 16384);
            labels.Add("KBD", 24576);
            labels.Add("SP", 0);
            labels.Add("LCL", 1);
            labels.Add("ARG", 2);
            labels.Add("THIS", 3);
            labels.Add("THAT", 4);
            return labels;
        }

        public static string[] Assemble(string[] input)
        {
            if (!_initialised) Initialise();

            List<string> output = new List<string>();
            Dictionary<string, int> labels = InitLabels();
            int index = -1;
            List<string> commands = new List<string>(input);

            int i = 0;
            while (i < commands.Count)
            {
                // PREPARE COMMAND
                string command = commands[i].Trim().Replace(" ", "").ToUpper();
                if (command == "")
                {
                    commands.RemoveAt(i);
                    continue;
                }

                // LABELS
                if (command[0] == '(')
                {
                    if (command.Length <= 2) return GenerateError(i);
                    if (command[command.Length - 1] != ')') return GenerateError(i);
                    string label = command.Substring(1, command.Length - 2);
                    if (labels.ContainsKey(label)) return GenerateError(i);
                    if (int.TryParse(label, out int _)) return GenerateError(i);
                    labels.Add(label, i);
                    commands.RemoveAt(i);
                    continue;
                }

                commands[i] = command;
                i++;
            }

            foreach (string command in commands)
            {
                index++;

                // A INSTRUCTION
                if (command[0] == '@')
                {
                    if (command.Length == 1) return GenerateError(index);
                    string strNum = command.Substring(1);

                    if (labels.ContainsKey(strNum))
                    {
                        int aLine = labels[strNum];
                        output.Add(GenerateACommand(aLine));
                        continue;
                    }

                    if (!int.TryParse(strNum, out int result)) return GenerateError(index);
                    if (result < 0 || result > 32767) return GenerateError(index);
                    output.Add(GenerateACommand(result));
                    continue;
                }

                // C INSTRUCTION

                string comp = "0000000";
                string dest = "000";
                string jump = "000";

                if (command.Contains('='))
                {
                    string[] parts = command.Split('=');
                    if (!_dest.ContainsKey(parts[0])) return GenerateError(index);
                    dest = _dest[parts[0]];

                    if (parts[1].Contains(';'))
                    {
                        string[] subParts = parts[1].Split(';');
                        if (!_comp.ContainsKey(subParts[0])) return GenerateError(index);
                        if (!_jump.ContainsKey(subParts[1])) return GenerateError(index);
                        comp = _comp[subParts[0]];
                        jump = _jump[subParts[1]];
                    }
                    else
                    {
                        if (!_comp.ContainsKey(parts[1])) return GenerateError(index);
                        comp = _comp[parts[1]];
                    }
                }
                else if(command.Contains(';'))
                {
                    string[] parts = command.Split(';');
                    if (!_comp.ContainsKey(parts[0])) return GenerateError(index);
                    if (!_jump.ContainsKey(parts[1])) return GenerateError(index);
                    comp = _comp[parts[0]];
                    jump = _jump[parts[1]];
                }
                else
                {
                    if (!_comp.ContainsKey(command)) return GenerateError(index);
                    comp = _comp[command];
                }

                output.Add($"111{comp}{dest}{jump}");
            }

            return output.ToArray();
        }

        private static string GenerateACommand(int number)
        {
            string output = "0";
            int binaryPlace = 16384;
            int workingNum = number;

            while (binaryPlace > 0)
            {
                if (workingNum >= binaryPlace)
                {
                    output += "1";
                    workingNum -= binaryPlace;
                }
                else
                {
                    output += "0";
                }
                binaryPlace /= 2;
            }

            return output;
        }

        private static string[] GenerateError(int line) => new string[] { $"ERROR ON LINE {line}" };
    }
}
