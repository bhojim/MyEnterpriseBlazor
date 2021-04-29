using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Tag
{
    public partial class TagDetail : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public ITagService TagService { get; set; }

        [Inject]
        public INavigationService NavigationService { get; set; }

        public TagModel Tag { get; set; } = new TagModel();

        protected override async Task OnInitializedAsync()
        {
            if (Id != 0)
            {
                Tag = await TagService.Get(Id.ToString());
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }
    }
}
