namespace Entities.Exceptions
{
    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("User was not authorized.")
        {
        }
    }
}