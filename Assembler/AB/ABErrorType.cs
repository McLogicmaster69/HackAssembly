namespace Assembler.AB
{
    public enum ABErrorType
    {
        None,
        MissingArguements,
        TooManyArguements,
        AlreadyExistingVariable,
        InvalidVariableIdentifier,
        InvalidAssignment,
        UnknownOperator,
        UnknownCommand,
        NoCorrespondingStatement,
        MissingEndStatement,
        VariableDoesNotExist
    }
}
