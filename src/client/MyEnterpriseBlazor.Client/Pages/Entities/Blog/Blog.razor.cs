using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal.Services;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Blog
{
    public partial class Blog : ComponentBase
    {
        [Inject]
        private IBlogService BlogService { get; set; }

        [CascadingParameter]
        private IModalService ModalService { get; set; }

        private IList<BlogModel> Blogs { get; set; } = new List<BlogModel>();

        protected override async Task OnInitializedAsync()
        {
            Blogs = await BlogService.GetAll();
        }

        private async Task Delete(int blogsId)
        {
            var deleteModal = ModalService.Show<DeleteModal>("Confirm delete operation");
            var deleteResult = await deleteModal.Result;
            if (!deleteResult.Cancelled)
            {
                await BlogService.Delete(blogsId.ToString());
                Blogs.Remove(Blogs.First(blogs => blogs.Id.Equals(blogsId)));
            }
        }
    }
}
