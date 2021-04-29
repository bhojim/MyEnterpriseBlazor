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
    public class TagRepository : GenericRepository<Tag>, ITagRepository
    {
        public TagRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<Tag> CreateOrUpdateAsync(Tag tag)
        {
            bool exists = await Exists(x => x.Id == tag.Id);

            if (tag.Id != 0 && exists)
            {
                Update(tag);
            }
            else
            {
                _context.AddOrUpdateGraph(tag);
            }
            return tag;
        }
    }
}
