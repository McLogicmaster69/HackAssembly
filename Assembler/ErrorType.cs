namespace Assembler
{
    public enum ErrorType
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
