using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JHipsterNet.Core.Pagination;
using JHipsterNet.Core.Pagination.Extensions;
using MyEnterpriseBlazor.Domain;
using MyEnterpriseBlazor.Domain.Repositories.Interfaces;
using MyEnterpriseBlazor.Infrastructure.Data.Extensions;

namespace MyEnterpriseBlazor.Infrastructure.Data.Repositories
{
    public class EntryRepository : GenericRepository<Entry>, IEntryRepository
    {
        public EntryRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<Entry> CreateOrUpdateAsync(Entry entry)
        {
            bool exists = await Exists(x => x.Id == entry.Id);

            await RemoveManyToManyRelationship("EntryTags", "EntriesId", "TagsId", entry.Id, entry.Tags.Select(x => x.Id).ToList());

            if (entry.Id != 0 && exists)
            {
                Update(entry);
            }
            else
            {
                _context.AddOrUpdateGraph(entry);
            }
            return entry;
        }
    }
}
