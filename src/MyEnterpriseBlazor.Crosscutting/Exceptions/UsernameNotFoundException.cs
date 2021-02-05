using System.Security.Authentication;

namespace MyEnterpriseBlazor.Crosscutting.Exceptions
{
    public class UsernameNotFoundException : AuthenticationException
    {
        public UsernameNotFoundException(string message) : base(message)
        {
        }
    }
}
