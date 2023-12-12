namespace Pronia.Utilities.Exceptions
{
    public class BadRequestException:Exception
    {
        public BadRequestException(string mess="Bad Request!"): base(mess)
        {
            
        }
    }
}
