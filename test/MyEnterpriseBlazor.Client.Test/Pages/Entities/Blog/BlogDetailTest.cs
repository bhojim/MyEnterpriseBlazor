using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Pages.Entities.Blog;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Blog
{
    public class BlogDetailTest : TestContext
    {
        private readonly Mock<IBlogService> _blogService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public BlogDetailTest()
        {
            _blogService = new Mock<IBlogService>();
            _navigationService = new Mock<INavigationService>();
            Services.AddSingleton<IBlogService>(_blogService.Object);
            Services.AddSingleton<INavigationService>(_navigationService.Object);
            Services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
            //This code is needed to support recursion
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        }

        [Fact]
        public void Should_DisplayBlog_When_IdIsPresent()
        {
            //Arrange
            var blog = _fixture.Create<BlogModel>();
            _blogService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(blog));
            var blogDetail = RenderComponent<BlogDetail>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var title = blogDetail.Find("h2");

            // Assert
            title.MarkupMatches($"<h2><span>Blog</span>{blog.Id}</h2>");

        }

        [Fact]
        public void Should_NotDisplayBlog_When_IdIsNotPresent()
        {
            //Arrange
            _blogService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(new BlogModel()));
            var blogDetail = RenderComponent<BlogDetail>();

            // Act
            var title = blogDetail.Find("div.col-8");

            // Assert
            title.Children.Length.Should().Be(0);

        }
    }
}
