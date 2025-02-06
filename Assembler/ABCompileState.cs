using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class ABCompileState
    {
        private Dictionary<string, int> _variables;

        private const int STARTING_VARIABLE_MEMORY_LOCATION = 16;

        public ABCompileState()
        {
            _variables = new Dictionary<string, int>();
        }

        public ErrorType Declare(string name)
        {
            if (_variables.ContainsKey(name)) return ErrorType.AlreadyExistingVariable;
            if (string.IsNullOrEmpty(name)) return ErrorType.InvalidVariableIdentifier;
            if (int.TryParse(name, out int _)) return ErrorType.InvalidVariableIdentifier;
            _variables.Add(name, STARTING_VARIABLE_MEMORY_LOCATION + _variables.Count);
            return ErrorType.None;
        }

        public bool ContainsVariable(string name) => _variables.ContainsKey(name);

        public string VariableMemoryAddress(string name) => _variables[name].ToString();
    }
}
