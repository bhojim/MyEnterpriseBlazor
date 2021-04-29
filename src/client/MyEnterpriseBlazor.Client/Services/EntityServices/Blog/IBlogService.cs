using System.Collections.Generic;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Blog
{
    public interface IBlogService
    {
        public Task<IList<BlogModel>> GetAll();

        public Task<BlogModel> Get(string id);

        public Task Add(BlogModel model);

        Task Update(BlogModel model);

        Task Delete(string id);
    }
}
