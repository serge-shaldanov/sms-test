namespace SmsTest.OrdersApi.Contracts.Exceptions;

public class OperationFailedException : Exception
{
    public OperationFailedException(string message) : base(message)
    {
    }

    public OperationFailedException(string message, Exception inner) : base(message, inner)
    {
    }
}
