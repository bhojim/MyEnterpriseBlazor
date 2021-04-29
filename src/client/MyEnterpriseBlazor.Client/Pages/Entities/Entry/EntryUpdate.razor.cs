using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using Microsoft.AspNetCore.Components;

namespace MyEnterpriseBlazor.Client.Pages.Entities.Entry
{
    public partial class EntryUpdate : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private IEntryService EntryService { get; set; }

        [Inject]
        private INavigationService NavigationService { get; set; }

        [Inject]
        private IBlogService BlogService { get; set; }

        [Inject]
        private ITagService TagService { get; set; }

        private IEnumerable<BlogModel> Blogs { get; set; } = new List<BlogModel>();

        private IEnumerable<TagModel> Tags { get; set; } = new List<TagModel>();

        public EntryModel EntryModel { get; set; } = new EntryModel();

        public IEnumerable<int> BlogIds { get; set; } = new List<int>();

        public int? BlogId { get; set; }

        public IReadOnlyList<int> TagIds { get; set; } = new List<int>();

        public IReadOnlyList<int> SelectedTags { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Blogs = await BlogService.GetAll();
            BlogIds = Blogs.Select(blog => blog.Id).ToList();
            Tags = await TagService.GetAll();
            TagIds = Tags.Select(tag => tag.Id).ToList();
            if (Id != 0)
            {
                EntryModel = await EntryService.Get(Id.ToString());
                BlogId = EntryModel.BlogId;
                SelectedTags = new List<int>(EntryModel.Tags.Select(tag => tag.Id));
            }
        }

        private void Back()
        {
            NavigationService.Previous();
        }

        private async Task Save()
        {
            EntryModel.BlogId = Blogs?.SingleOrDefault(blog => blog.Id.Equals(BlogId))?.Id;
            if (SelectedTags != null)
            {
                EntryModel.Tags = Tags?.Where(tag => SelectedTags.Contains(tag.Id)).ToList();
            }
            else
            {
                EntryModel.Tags = null;
            }
            if (Id != 0)
            {
                await EntryService.Update(EntryModel);
            }
            else
            {
                await EntryService.Add(EntryModel);
            }
            NavigationService.Previous();
        }
    }
}
