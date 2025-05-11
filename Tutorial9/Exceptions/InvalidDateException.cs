namespace Tutorial9.Exceptions;

public class InvalidDateException : Exception
{
    public InvalidDateException()
        : base("CreatedAt date of the request should be higher then Created date of the Order") { }
}