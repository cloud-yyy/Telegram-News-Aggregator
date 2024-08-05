namespace Entities.Exceptions
{
    public class EnviromentVariableNotFoundException : Exception
    {
        public EnviromentVariableNotFoundException(string name)
            : base($"Enviroment variable {name} was not found.`")
        {
        }        
    }
}