using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.AB
{
    public class ABCompileState
    {
        private Dictionary<string, int> _variables;
        private List<int> _availableMemory;

        private const int STARTING_VARIABLE_MEMORY_LOCATION = 16;

        public ABCompileState()
        {
            _variables = new Dictionary<string, int>();
            _availableMemory = new List<int>();
        }

        public ABErrorType Declare(string name, out bool requireInit)
        {
            requireInit = false;
            if (_variables.ContainsKey(name)) return ABErrorType.AlreadyExistingVariable;
            if (string.IsNullOrEmpty(name)) return ABErrorType.InvalidVariableIdentifier;
            if (int.TryParse(name, out int _)) return ABErrorType.InvalidVariableIdentifier;
            if (_availableMemory.Count > 0)
            {
                requireInit = true;
                _variables.Add(name, _availableMemory[0]);
                _availableMemory.RemoveAt(0);
            }
            else
            {
                _variables.Add(name, STARTING_VARIABLE_MEMORY_LOCATION + _variables.Count);
            }
            return ABErrorType.None;
        }

        public ABErrorType Remove(string name)
        {
            if (!_variables.ContainsKey(name)) return ABErrorType.VariableDoesNotExist;
            int memoryLocation = _variables[name];
            _variables.Remove(name);
            _availableMemory.Add(memoryLocation);
            return ABErrorType.None;
        }

        public bool ContainsVariable(string name) => _variables.ContainsKey(name);

        public string VariableMemoryAddress(string name) => _variables[name].ToString();
    }
}
