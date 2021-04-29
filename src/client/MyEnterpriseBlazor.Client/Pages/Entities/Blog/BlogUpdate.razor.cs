using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Services.EntityServices.User;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Blog
{
    public partial class BlogUpdate : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private IBlogService BlogService { get; set; }

        [Inject]
        private INavigationService NavigationService { get; set; }

        [Inject]
        private IUserService UserService { get; set; }

        private IEnumerable<UserModel> Users { get; set; } = new List<UserModel>();

        public BlogModel BlogModel { get; set; } = new BlogModel();

        public IEnumerable<string> UserIds { get; set; } = new List<string>();

        public string UserId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Users = await UserService.GetAll();
            UserIds = Users.Select(user => user.Id).ToList();
            if (Id != 0)
            {
                BlogModel = await BlogService.Get(Id.ToString());
                UserId = BlogModel.UserId;
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }

        private async Task Save()
        {
            BlogModel.UserId = Users?.SingleOrDefault(user => user.Id.Equals(UserId))?.Id;
            if (Id != 0)
            {
                await BlogService.Update(BlogModel);
            }
            else
            {
                await BlogService.Add(BlogModel);
            }
            NavigationService.Previous();
        }
    }
}
