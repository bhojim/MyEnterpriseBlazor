using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Tag
{
    public partial class TagUpdate : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private ITagService TagService { get; set; }

        [Inject]
        private INavigationService NavigationService { get; set; }

        public TagModel TagModel { get; set; } = new TagModel();

        protected override async Task OnInitializedAsync()
        {
            if (Id != 0)
            {
                TagModel = await TagService.Get(Id.ToString());
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }

        private async Task Save()
        {
            if (Id != 0)
            {
                await TagService.Update(TagModel);
            }
            else
            {
                await TagService.Add(TagModel);
            }
            NavigationService.Previous();
        }
    }
}
