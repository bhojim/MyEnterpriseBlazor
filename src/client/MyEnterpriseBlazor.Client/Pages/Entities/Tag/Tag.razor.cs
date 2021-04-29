using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Tag
{
    public partial class Tag : ComponentBase
    {
        [Inject]
        private ITagService TagService { get; set; }

        [CascadingParameter]
        private IModalService ModalService { get; set; }

        private IList<TagModel> Tags { get; set; } = new List<TagModel>();

        protected override async Task OnInitializedAsync()
        {
            Tags = await TagService.GetAll();
        }

        private async Task Delete(int tagsId)
        {
            var deleteModal = ModalService.Show<DeleteModal>("Confirm delete operation");
            var deleteResult = await deleteModal.Result;
            if (!deleteResult.Cancelled)
            {
                await TagService.Delete(tagsId.ToString());
                Tags.Remove(Tags.First(tags => tags.Id.Equals(tagsId)));
            }
        }
    }
}
