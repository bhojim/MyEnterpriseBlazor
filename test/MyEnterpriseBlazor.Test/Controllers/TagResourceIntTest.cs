
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using MyEnterpriseBlazor.Infrastructure.Data;
using MyEnterpriseBlazor.Domain;
using MyEnterpriseBlazor.Domain.Repositories.Interfaces;
using MyEnterpriseBlazor.Test.Setup;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MyEnterpriseBlazor.Test.Controllers
{
    public class TagResourceIntTest
    {
        public TagResourceIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _tagRepository = _factory.GetRequiredService<ITagRepository>();


            InitTest();
        }

        private const string DefaultName = "AAAAAAAAAA";
        private const string UpdatedName = "BBBBBBBBBB";

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly ITagRepository _tagRepository;

        private Tag _tag;


        private Tag CreateEntity()
        {
            return new Tag
            {
                Name = DefaultName
            };
        }

        private void InitTest()
        {
            _tag = CreateEntity();
        }

        [Fact]
        public async Task CreateTag()
        {
            var databaseSizeBeforeCreate = await _tagRepository.CountAsync();

            // Create the Tag
            var response = await _client.PostAsync("/api/tags", TestUtil.ToJsonContent(_tag));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Tag in the database
            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testTag = tagList.Last();
            testTag.Name.Should().Be(DefaultName);
        }

        [Fact]
        public async Task CreateTagWithExistingId()
        {
            var databaseSizeBeforeCreate = await _tagRepository.CountAsync();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Tag with an existing ID
            _tag.Id = 1;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/tags", TestUtil.ToJsonContent(_tag));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Tag in the database
            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckNameIsRequired()
        {
            var databaseSizeBeforeTest = await _tagRepository.CountAsync();

            // Set the field to null
            _tag.Name = null;

            // Create the Tag, which fails.
            var response = await _client.PostAsync("/api/tags", TestUtil.ToJsonContent(_tag));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllTags()
        {
            // Initialize the database
            await _tagRepository.CreateOrUpdateAsync(_tag);
            await _tagRepository.SaveChangesAsync();

            // Get all the tagList
            var response = await _client.GetAsync("/api/tags?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_tag.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetTag()
        {
            // Initialize the database
            await _tagRepository.CreateOrUpdateAsync(_tag);
            await _tagRepository.SaveChangesAsync();

            // Get the tag
            var response = await _client.GetAsync($"/api/tags/{_tag.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_tag.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
        }

        [Fact]
        public async Task GetNonExistingTag()
        {
            var response = await _client.GetAsync("/api/tags/" + int.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateTag()
        {
            // Initialize the database
            await _tagRepository.CreateOrUpdateAsync(_tag);
            await _tagRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _tagRepository.CountAsync();

            // Update the tag
            var updatedTag = await _tagRepository.QueryHelper().GetOneAsync(it => it.Id == _tag.Id);
            // Disconnect from session so that the updates on updatedTag are not directly saved in db
            //TODO detach
            updatedTag.Name = UpdatedName;

            var response = await _client.PutAsync("/api/tags", TestUtil.ToJsonContent(updatedTag));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Tag in the database
            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testTag = tagList.Last();
            testTag.Name.Should().Be(UpdatedName);
        }

        [Fact]
        public async Task UpdateNonExistingTag()
        {
            var databaseSizeBeforeUpdate = await _tagRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/tags", TestUtil.ToJsonContent(_tag));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Tag in the database
            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteTag()
        {
            // Initialize the database
            await _tagRepository.CreateOrUpdateAsync(_tag);
            await _tagRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _tagRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/tags/{_tag.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var tagList = await _tagRepository.GetAllAsync();
            tagList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Tag));
            var tag1 = new Tag
            {
                Id = 1
            };
            var tag2 = new Tag
            {
                Id = tag1.Id
            };
            tag1.Should().Be(tag2);
            tag2.Id = 2;
            tag1.Should().NotBe(tag2);
            tag1.Id = 0;
            tag1.Should().NotBe(tag2);
        }
    }
}
