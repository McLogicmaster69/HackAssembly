namespace Assembler.VM
{
    public enum VMErrorType
    {
        None,
        UnknownCommand,
        MissingArguements,
        TooManyArguements,
        InvalidScope,
        InvalidAssignment
    }
}
