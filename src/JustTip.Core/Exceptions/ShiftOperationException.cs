namespace JustTip.Core.Exceptions
{
    public class ShiftOperationException : Exception
    {
        public ShiftOperationException(string message)
            : base(message)
        {
        }

        public ShiftOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 