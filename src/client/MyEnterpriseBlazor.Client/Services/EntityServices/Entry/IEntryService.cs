using System.Collections.Generic;
using System.Threading.Tasks;
using MyEnterpriseBlazor.Client.Models;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Entry
{
    public interface IEntryService
    {
        public Task<IList<EntryModel>> GetAll();

        public Task<EntryModel> Get(string id);

        public Task Add(EntryModel model);

        Task Update(EntryModel model);

        Task Delete(string id);
    }
}
