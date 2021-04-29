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
    public class BlogRepository : GenericRepository<Blog>, IBlogRepository
    {
        public BlogRepository(IUnitOfWork context) : base(context)
        {
        }

        public override async Task<Blog> CreateOrUpdateAsync(Blog blog)
        {
            bool exists = await Exists(x => x.Id == blog.Id);

            if (blog.Id != 0 && exists)
            {
                Update(blog);
            }
            else
            {
                _context.AddOrUpdateGraph(blog);
            }
            return blog;
        }
    }
}
