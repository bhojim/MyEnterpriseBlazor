using System;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;

namespace MyEnterpriseBlazor.Client.Services
{
    public interface IAuthenticationService
    {
        public bool IsAuthenticated { get; set; }
        public UserModel CurrentUser { get; set; }
        public JwtToken JwtToken { get; set; }

        Task<bool> SignIn(LoginModel loginModel);
        public Task SignOut();

    }
}
