
using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using MyEnterpriseBlazor.Domain;
using MyEnterpriseBlazor.Crosscutting.Exceptions;
using MyEnterpriseBlazor.Domain.Repositories.Interfaces;
using MyEnterpriseBlazor.Web.Extensions;
using MyEnterpriseBlazor.Web.Filters;
using MyEnterpriseBlazor.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace MyEnterpriseBlazor.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private const string EntityName = "blog";
        private readonly IBlogRepository _blogRepository;
        private readonly ILogger<BlogController> _log;

        public BlogController(ILogger<BlogController> log,
            IBlogRepository blogRepository)
        {
            _log = log;
            _blogRepository = blogRepository;
        }

        [HttpPost("blogs")]
        [ValidateModel]
        public async Task<ActionResult<Blog>> CreateBlog([FromBody] Blog blog)
        {
            _log.LogDebug($"REST request to save Blog : {blog}");
            if (blog.Id != 0)
                throw new BadRequestAlertException("A new blog cannot already have an ID", EntityName, "idexists");

            await _blogRepository.CreateOrUpdateAsync(blog);
            await _blogRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, blog.Id.ToString()));
        }

        [HttpPut("blogs")]
        [ValidateModel]
        public async Task<IActionResult> UpdateBlog([FromBody] Blog blog)
        {
            _log.LogDebug($"REST request to update Blog : {blog}");
            if (blog.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            await _blogRepository.CreateOrUpdateAsync(blog);
            await _blogRepository.SaveChangesAsync();
            return Ok(blog)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, blog.Id.ToString()));
        }

        [HttpGet("blogs")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogs(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Blogs");
            var result = await _blogRepository.QueryHelper()
                .Include(blog => blog.User)
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        [HttpGet("blogs/{id}")]
        public async Task<IActionResult> GetBlog([FromRoute] int id)
        {
            _log.LogDebug($"REST request to get Blog : {id}");
            var result = await _blogRepository.QueryHelper()
                .Include(blog => blog.User)
                .GetOneAsync(blog => blog.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("blogs/{id}")]
        public async Task<IActionResult> DeleteBlog([FromRoute] int id)
        {
            _log.LogDebug($"REST request to delete Blog : {id}");
            await _blogRepository.DeleteByIdAsync(id);
            await _blogRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
