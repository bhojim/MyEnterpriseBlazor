using System;

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
    public class EntryResourceIntTest
    {
        public EntryResourceIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _entryRepository = _factory.GetRequiredService<IEntryRepository>();


            InitTest();
        }

        private const string DefaultTitle = "AAAAAAAAAA";
        private const string UpdatedTitle = "BBBBBBBBBB";

        private static readonly string DefaultContent = "";
        private static readonly string UpdatedContent = "";

        private static readonly DateTime DefaultDate = DateTime.UnixEpoch;
        private static readonly DateTime UpdatedDate = DateTime.Now;

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly IEntryRepository _entryRepository;

        private Entry _entry;


        private Entry CreateEntity()
        {
            return new Entry
            {
                Title = DefaultTitle,
                Content = DefaultContent,
                Date = DefaultDate
            };
        }

        private void InitTest()
        {
            _entry = CreateEntity();
        }

        [Fact]
        public async Task CreateEntry()
        {
            var databaseSizeBeforeCreate = await _entryRepository.CountAsync();

            // Create the Entry
            var response = await _client.PostAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Entry in the database
            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testEntry = entryList.Last();
            testEntry.Title.Should().Be(DefaultTitle);
            testEntry.Content.Should().Be(DefaultContent);
            testEntry.Date.Should().Be(DefaultDate);
        }

        [Fact]
        public async Task CreateEntryWithExistingId()
        {
            var databaseSizeBeforeCreate = await _entryRepository.CountAsync();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Entry with an existing ID
            _entry.Id = 1;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Entry in the database
            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckTitleIsRequired()
        {
            var databaseSizeBeforeTest = await _entryRepository.CountAsync();

            // Set the field to null
            _entry.Title = null;

            // Create the Entry, which fails.
            var response = await _client.PostAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task CheckContentIsRequired()
        {
            var databaseSizeBeforeTest = await _entryRepository.CountAsync();

            // Set the field to null
            _entry.Content = null;

            // Create the Entry, which fails.
            var response = await _client.PostAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task CheckDateIsRequired()
        {
            var databaseSizeBeforeTest = await _entryRepository.CountAsync();

            // Set the field to null
            _entry.Date = null;

            // Create the Entry, which fails.
            var response = await _client.PostAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllEntries()
        {
            // Initialize the database
            await _entryRepository.CreateOrUpdateAsync(_entry);
            await _entryRepository.SaveChangesAsync();

            // Get all the entryList
            var response = await _client.GetAsync("/api/entries?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_entry.Id);
            json.SelectTokens("$.[*].title").Should().Contain(DefaultTitle);
            json.SelectTokens("$.[*].content").Should().Contain(DefaultContent);
            json.SelectTokens("$.[*].date").Should().Contain(DefaultDate);
        }

        [Fact]
        public async Task GetEntry()
        {
            // Initialize the database
            await _entryRepository.CreateOrUpdateAsync(_entry);
            await _entryRepository.SaveChangesAsync();

            // Get the entry
            var response = await _client.GetAsync($"/api/entries/{_entry.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_entry.Id);
            json.SelectTokens("$.title").Should().Contain(DefaultTitle);
            json.SelectTokens("$.content").Should().Contain(DefaultContent);
            json.SelectTokens("$.date").Should().Contain(DefaultDate);
        }

        [Fact]
        public async Task GetNonExistingEntry()
        {
            var response = await _client.GetAsync("/api/entries/" + int.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateEntry()
        {
            // Initialize the database
            await _entryRepository.CreateOrUpdateAsync(_entry);
            await _entryRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _entryRepository.CountAsync();

            // Update the entry
            var updatedEntry = await _entryRepository.QueryHelper().GetOneAsync(it => it.Id == _entry.Id);
            // Disconnect from session so that the updates on updatedEntry are not directly saved in db
            //TODO detach
            updatedEntry.Title = UpdatedTitle;
            updatedEntry.Content = UpdatedContent;
            updatedEntry.Date = UpdatedDate;

            var response = await _client.PutAsync("/api/entries", TestUtil.ToJsonContent(updatedEntry));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Entry in the database
            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testEntry = entryList.Last();
            testEntry.Title.Should().Be(UpdatedTitle);
            testEntry.Content.Should().Be(UpdatedContent);
            testEntry.Date.Should().Be(UpdatedDate);
        }

        [Fact]
        public async Task UpdateNonExistingEntry()
        {
            var databaseSizeBeforeUpdate = await _entryRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/entries", TestUtil.ToJsonContent(_entry));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Entry in the database
            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteEntry()
        {
            // Initialize the database
            await _entryRepository.CreateOrUpdateAsync(_entry);
            await _entryRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _entryRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/entries/{_entry.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var entryList = await _entryRepository.GetAllAsync();
            entryList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Entry));
            var entry1 = new Entry
            {
                Id = 1
            };
            var entry2 = new Entry
            {
                Id = entry1.Id
            };
            entry1.Should().Be(entry2);
            entry2.Id = 2;
            entry1.Should().NotBe(entry2);
            entry1.Id = 0;
            entry1.Should().NotBe(entry2);
        }
    }
}
