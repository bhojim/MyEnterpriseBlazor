using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Entry
{
    public partial class EntryDetail : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IEntryService EntryService { get; set; }

        [Inject]
        public INavigationService NavigationService { get; set; }

        public EntryModel Entry { get; set; } = new EntryModel();

        protected override async Task OnInitializedAsync()
        {
            if (Id != 0)
            {
                Entry = await EntryService.Get(Id.ToString());
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }
    }
}
