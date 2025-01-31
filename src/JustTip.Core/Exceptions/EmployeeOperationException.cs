namespace JustTip.Core.Exceptions
{
    public class EmployeeOperationException : Exception
    {
        public EmployeeOperationException(string message)
            : base(message)
        {
        }

        public EmployeeOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 