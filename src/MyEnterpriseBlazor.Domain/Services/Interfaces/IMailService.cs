using System.Threading.Tasks;
using MyEnterpriseBlazor.Domain;

namespace MyEnterpriseBlazor.Domain.Services.Interfaces
{
    public interface IMailService
    {
        Task SendPasswordResetMail(User user);
        Task SendActivationEmail(User user);
        Task SendCreationEmail(User user);
    }
}
