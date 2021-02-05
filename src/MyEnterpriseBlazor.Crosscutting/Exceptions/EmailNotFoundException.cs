using MyEnterpriseBlazor.Crosscutting.Constants;

namespace MyEnterpriseBlazor.Crosscutting.Exceptions
{
    public class EmailNotFoundException : BaseException
    {
        public EmailNotFoundException() : base(ErrorConstants.EmailNotFoundType, "Email address not registered")
        {
        }
    }
}
