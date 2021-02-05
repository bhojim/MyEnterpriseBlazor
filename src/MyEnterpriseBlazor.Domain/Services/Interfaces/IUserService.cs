using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Crosscutting.Constants;
using MyEnterpriseBlazor.Crosscutting.Exceptions;
using MyEnterpriseBlazor.Domain;

namespace MyEnterpriseBlazor.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(User userToCreate);
        IEnumerable<string> GetAuthorities();
        Task DeleteUser(string login);
        Task<User> UpdateUser(User userToUpdate);
        Task<User> CompletePasswordReset(string newPassword, string key);
        Task<User> RequestPasswordReset(string mail);
        Task ChangePassword(string currentClearTextPassword, string newPassword);
        Task<User> ActivateRegistration(string key);
        Task<User> RegisterUser(User userToRegister, string password);
        Task UpdateUser(string firstName, string lastName, string email, string langKey, string imageUrl);
        Task<User> GetUserWithUserRoles();
    }
}
