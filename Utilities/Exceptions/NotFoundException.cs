namespace Pronia.Utilities.Exceptions
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string mess= "Not found!") : base(mess)
        {
        }

     
    }
}
