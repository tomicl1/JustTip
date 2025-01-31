namespace JustTip.Core.Exceptions
{
    public class BusinessNotFoundException : Exception
    {
        public BusinessNotFoundException(int businessId)
            : base($"Business with ID {businessId} not found.")
        {
        }
    }
} 