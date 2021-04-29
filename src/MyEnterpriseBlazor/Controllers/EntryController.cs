
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
    public class EntryController : ControllerBase
    {
        private const string EntityName = "entry";
        private readonly IEntryRepository _entryRepository;
        private readonly ILogger<EntryController> _log;

        public EntryController(ILogger<EntryController> log,
            IEntryRepository entryRepository)
        {
            _log = log;
            _entryRepository = entryRepository;
        }

        [HttpPost("entries")]
        [ValidateModel]
        public async Task<ActionResult<Entry>> CreateEntry([FromBody] Entry entry)
        {
            _log.LogDebug($"REST request to save Entry : {entry}");
            if (entry.Id != 0)
                throw new BadRequestAlertException("A new entry cannot already have an ID", EntityName, "idexists");

            await _entryRepository.CreateOrUpdateAsync(entry);
            await _entryRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntry), new { id = entry.Id }, entry)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, entry.Id.ToString()));
        }

        [HttpPut("entries")]
        [ValidateModel]
        public async Task<IActionResult> UpdateEntry([FromBody] Entry entry)
        {
            _log.LogDebug($"REST request to update Entry : {entry}");
            if (entry.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            await _entryRepository.CreateOrUpdateAsync(entry);
            await _entryRepository.SaveChangesAsync();
            return Ok(entry)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, entry.Id.ToString()));
        }

        [HttpGet("entries")]
        public async Task<ActionResult<IEnumerable<Entry>>> GetAllEntries(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Entries");
            var result = await _entryRepository.QueryHelper()
                .Include(entry => entry.Blog)
                .Include(entry => entry.Tags)
                .GetPageAsync(pageable);
            return Ok(result.Content).WithHeaders(result.GeneratePaginationHttpHeaders());
        }

        [HttpGet("entries/{id}")]
        public async Task<IActionResult> GetEntry([FromRoute] int id)
        {
            _log.LogDebug($"REST request to get Entry : {id}");
            var result = await _entryRepository.QueryHelper()
                .Include(entry => entry.Blog)
                .Include(entry => entry.Tags)
                .GetOneAsync(entry => entry.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("entries/{id}")]
        public async Task<IActionResult> DeleteEntry([FromRoute] int id)
        {
            _log.LogDebug($"REST request to delete Entry : {id}");
            await _entryRepository.DeleteByIdAsync(id);
            await _entryRepository.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
