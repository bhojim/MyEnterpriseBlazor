using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Blog
{
    public partial class BlogDetail : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IBlogService BlogService { get; set; }

        [Inject]
        public INavigationService NavigationService { get; set; }

        public BlogModel Blog { get; set; } = new BlogModel();

        protected override async Task OnInitializedAsync()
        {
            if (Id != 0)
            {
                Blog = await BlogService.Get(Id.ToString());
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }
    }
}
