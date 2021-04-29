using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Entry
{
    public partial class Entry : ComponentBase
    {
        [Inject]
        private IEntryService EntryService { get; set; }

        [CascadingParameter]
        private IModalService ModalService { get; set; }

        private IList<EntryModel> Entries { get; set; } = new List<EntryModel>();

        protected override async Task OnInitializedAsync()
        {
            Entries = await EntryService.GetAll();
        }

        private async Task Delete(int entriesId)
        {
            var deleteModal = ModalService.Show<DeleteModal>("Confirm delete operation");
            var deleteResult = await deleteModal.Result;
            if (!deleteResult.Cancelled)
            {
                await EntryService.Delete(entriesId.ToString());
                Entries.Remove(Entries.First(entries => entries.Id.Equals(entriesId)));
            }
        }
    }
}
