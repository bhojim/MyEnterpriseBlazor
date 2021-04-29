
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
    public class TagController : ControllerBase
    {
        private const string EntityName = "tag";
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<TagController> _log;

        public TagController(ILogger<TagController> log,
            ITagRepository tagRepository)
        {
            _log = log;
            _tagRepository = tagRepository;
        }

        [HttpPost("tags")]
        [ValidateModel]
        public async Task<ActionResult<Tag>> CreateTag([FromBody] Tag tag)
        {
            _log.LogDebug($"REST request to save Tag : {tag}");
            if (tag.Id != 0)
                throw new BadRequestAlertException("A new tag cannot already have an ID", EntityName, "idexists");

            await _tagRepository.CreateOrUpdateAsync(tag);
            await _tagRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, tag.Id.ToString()));
        }

        [HttpPut("tags")]
        [ValidateModel]
        public async Task<IActionResult> UpdateTag([FromBody] Tag tag)
        {
            _log.LogDebug($"REST request to update Tag : {tag}");
            if (tag.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            await _tagRepository.CreateOrUpdateAsync(tag);
            await _tagRepository.SaveChangesAsync();
            return Ok(tag)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, tag.Id.ToString()));
        }

        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Tags");
            var result = await _tagRepository.QueryHelper()
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        [HttpGet("tags/{id}")]
        public async Task<IActionResult> GetTag([FromRoute] int id)
        {
            _log.LogDebug($"REST request to get Tag : {id}");
            var result = await _tagRepository.QueryHelper()
                .GetOneAsync(tag => tag.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag([FromRoute] int id)
        {
            _log.LogDebug($"REST request to delete Tag : {id}");
            await _tagRepository.DeleteByIdAsync(id);
            await _tagRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
