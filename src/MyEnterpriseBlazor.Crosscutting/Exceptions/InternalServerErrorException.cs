using MyEnterpriseBlazor.Crosscutting.Constants;

namespace MyEnterpriseBlazor.Crosscutting.Exceptions
{
    public class InternalServerErrorException : BaseException
    {
        public InternalServerErrorException(string message) : base(ErrorConstants.DefaultType, message)
        {
        }
    }
}
