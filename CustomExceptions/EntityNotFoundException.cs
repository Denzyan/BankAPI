namespace BankApiService.CustomExceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string? message) : base (message)
        { 
        }
    }
}
