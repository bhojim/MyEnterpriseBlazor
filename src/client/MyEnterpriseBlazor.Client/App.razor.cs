using Microsoft.AspNetCore.Components;
using MyEnterpriseBlazor.Client.Pages.Utils;

namespace MyEnterpriseBlazor.Client
{
    public partial class App : ComponentBase
    {
        [Inject]
        public INavigationService NavigationService { get; set; } // Permit to initialize navigation service
    }
}
