using System.Threading.Tasks;
using Blazored.Modal.Services;
using MyEnterpriseBlazor.Client.Pages;
using MyEnterpriseBlazor.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyEnterpriseBlazor.Client.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private AuthenticationStateProvider AuthenticationService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        private IModalService ModalService { get; set; }

        private async Task SignIn()
        {
            ModalService.Show<Login>("Sign In");
        }

        private async Task SignOut()
        {
            await (AuthenticationService as IAuthenticationService).SignOut();
            NavigationManager.NavigateTo("/");
        }

        private void UserManagement()
        {
            NavigationManager.NavigateTo("/admin/user-management");
        }

    }
}
