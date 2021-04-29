using System.Collections.Generic;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Tag
{
    public interface ITagService
    {
        public Task<IList<TagModel>> GetAll();

        public Task<TagModel> Get(string id);

        public Task Add(TagModel model);

        Task Update(TagModel model);

        Task Delete(string id);
    }
}
