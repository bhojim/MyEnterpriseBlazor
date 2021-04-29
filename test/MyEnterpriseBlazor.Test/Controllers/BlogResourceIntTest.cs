
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
    public class BlogResourceIntTest
    {
        public BlogResourceIntTest()
        {
            _factory = new AppWebApplicationFactory<TestStartup>().WithMockUser();
            _client = _factory.CreateClient();

            _blogRepository = _factory.GetRequiredService<IBlogRepository>();


            InitTest();
        }

        private const string DefaultName = "AAAAAAAAAA";
        private const string UpdatedName = "BBBBBBBBBB";

        private const string DefaultHandle = "AAAAAAAAAA";
        private const string UpdatedHandle = "BBBBBBBBBB";

        private readonly AppWebApplicationFactory<TestStartup> _factory;
        private readonly HttpClient _client;
        private readonly IBlogRepository _blogRepository;

        private Blog _blog;


        private Blog CreateEntity()
        {
            return new Blog
            {
                Name = DefaultName,
                Handle = DefaultHandle
            };
        }

        private void InitTest()
        {
            _blog = CreateEntity();
        }

        [Fact]
        public async Task CreateBlog()
        {
            var databaseSizeBeforeCreate = await _blogRepository.CountAsync();

            // Create the Blog
            var response = await _client.PostAsync("/api/blogs", TestUtil.ToJsonContent(_blog));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            // Validate the Blog in the database
            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeCreate + 1);
            var testBlog = blogList.Last();
            testBlog.Name.Should().Be(DefaultName);
            testBlog.Handle.Should().Be(DefaultHandle);
        }

        [Fact]
        public async Task CreateBlogWithExistingId()
        {
            var databaseSizeBeforeCreate = await _blogRepository.CountAsync();
            databaseSizeBeforeCreate.Should().Be(0);
            // Create the Blog with an existing ID
            _blog.Id = 1;

            // An entity with an existing ID cannot be created, so this API call must fail
            var response = await _client.PostAsync("/api/blogs", TestUtil.ToJsonContent(_blog));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Blog in the database
            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeCreate);
        }

        [Fact]
        public async Task CheckNameIsRequired()
        {
            var databaseSizeBeforeTest = await _blogRepository.CountAsync();

            // Set the field to null
            _blog.Name = null;

            // Create the Blog, which fails.
            var response = await _client.PostAsync("/api/blogs", TestUtil.ToJsonContent(_blog));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task CheckHandleIsRequired()
        {
            var databaseSizeBeforeTest = await _blogRepository.CountAsync();

            // Set the field to null
            _blog.Handle = null;

            // Create the Blog, which fails.
            var response = await _client.PostAsync("/api/blogs", TestUtil.ToJsonContent(_blog));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeTest);
        }

        [Fact]
        public async Task GetAllBlogs()
        {
            // Initialize the database
            await _blogRepository.CreateOrUpdateAsync(_blog);
            await _blogRepository.SaveChangesAsync();

            // Get all the blogList
            var response = await _client.GetAsync("/api/blogs?sort=id,desc");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.[*].id").Should().Contain(_blog.Id);
            json.SelectTokens("$.[*].name").Should().Contain(DefaultName);
            json.SelectTokens("$.[*].handle").Should().Contain(DefaultHandle);
        }

        [Fact]
        public async Task GetBlog()
        {
            // Initialize the database
            await _blogRepository.CreateOrUpdateAsync(_blog);
            await _blogRepository.SaveChangesAsync();

            // Get the blog
            var response = await _client.GetAsync($"/api/blogs/{_blog.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = JToken.Parse(await response.Content.ReadAsStringAsync());
            json.SelectTokens("$.id").Should().Contain(_blog.Id);
            json.SelectTokens("$.name").Should().Contain(DefaultName);
            json.SelectTokens("$.handle").Should().Contain(DefaultHandle);
        }

        [Fact]
        public async Task GetNonExistingBlog()
        {
            var response = await _client.GetAsync("/api/blogs/" + int.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateBlog()
        {
            // Initialize the database
            await _blogRepository.CreateOrUpdateAsync(_blog);
            await _blogRepository.SaveChangesAsync();
            var databaseSizeBeforeUpdate = await _blogRepository.CountAsync();

            // Update the blog
            var updatedBlog = await _blogRepository.QueryHelper().GetOneAsync(it => it.Id == _blog.Id);
            // Disconnect from session so that the updates on updatedBlog are not directly saved in db
            //TODO detach
            updatedBlog.Name = UpdatedName;
            updatedBlog.Handle = UpdatedHandle;

            var response = await _client.PutAsync("/api/blogs", TestUtil.ToJsonContent(updatedBlog));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the Blog in the database
            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeUpdate);
            var testBlog = blogList.Last();
            testBlog.Name.Should().Be(UpdatedName);
            testBlog.Handle.Should().Be(UpdatedHandle);
        }

        [Fact]
        public async Task UpdateNonExistingBlog()
        {
            var databaseSizeBeforeUpdate = await _blogRepository.CountAsync();

            // If the entity doesn't have an ID, it will throw BadRequestAlertException
            var response = await _client.PutAsync("/api/blogs", TestUtil.ToJsonContent(_blog));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            // Validate the Blog in the database
            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeUpdate);
        }

        [Fact]
        public async Task DeleteBlog()
        {
            // Initialize the database
            await _blogRepository.CreateOrUpdateAsync(_blog);
            await _blogRepository.SaveChangesAsync();
            var databaseSizeBeforeDelete = await _blogRepository.CountAsync();

            var response = await _client.DeleteAsync($"/api/blogs/{_blog.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Validate the database is empty
            var blogList = await _blogRepository.GetAllAsync();
            blogList.Count().Should().Be(databaseSizeBeforeDelete - 1);
        }

        [Fact]
        public void EqualsVerifier()
        {
            TestUtil.EqualsVerifier(typeof(Blog));
            var blog1 = new Blog
            {
                Id = 1
            };
            var blog2 = new Blog
            {
                Id = blog1.Id
            };
            blog1.Should().Be(blog2);
            blog2.Id = 2;
            blog1.Should().NotBe(blog2);
            blog1.Id = 0;
            blog1.Should().NotBe(blog2);
        }
    }
}
