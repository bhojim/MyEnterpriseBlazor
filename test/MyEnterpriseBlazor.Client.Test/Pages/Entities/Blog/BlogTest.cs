using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blazored.Modal;
using Blazored.Modal.Services;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Blog
{
    public class BlogTest : TestContext
    {
        private readonly Mock<IBlogService> _blogService;
        private readonly Mock<IModalService> _modalService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public BlogTest()
        {
            _blogService = new Mock<IBlogService>();
            _modalService = new Mock<IModalService>();
            Services.AddSingleton<IBlogService>(_blogService.Object);
            Services.AddSingleton<IModalService>(_modalService.Object);
            Services.AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
            Services.AddHttpClientInterceptor();
            //This code is needed to support recursion
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        }

        [Fact]
        public void Should_DisplayAllBlogs_When_BlogsArePresent()
        {
            //Arrange
            var blogs = _fixture.CreateMany<BlogModel>(10);
            _blogService.Setup(service => service.GetAll()).Returns(Task.FromResult(blogs.ToList() as IList<BlogModel>));
            var blogPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Blog.Blog>();

            // Act
            var blogsTableBody = blogPage.Find("tbody");

            // Assert
            blogsTableBody.ChildElementCount.Should().Be(10);
        }

        [Fact]
        public void Should_DisplayNoCountry_When_BlogsLengthIsZero()
        {
            //Arrange
            var blogs = new List<BlogModel>();
            _blogService.Setup(service => service.GetAll()).Returns(Task.FromResult(blogs.ToList() as IList<BlogModel>));
            var blogPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Blog.Blog>();

            // Act
            var span = blogPage.Find("div>span");

            // Assert
            span.MarkupMatches("<span>No Blogs found</span>");
        }

        [Fact]
        public void Should_DeleteBlog_WhenDeleteButtonClicked()
        {
            //Arrange
            var blogs = _fixture.CreateMany<BlogModel>(10);
            _blogService.Setup(service => service.GetAll()).Returns(Task.FromResult(blogs.ToList() as IList<BlogModel>));

            var modalRef = new Mock<IModalReference>();
            modalRef.Setup(mock => mock.Result).Returns(Task.FromResult(ModalResult.Ok(new { })));
            _modalService.Setup(service => service.Show<DeleteModal>(It.IsAny<string>())).Returns(modalRef.Object);
            var blogPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Blog.Blog>(ComponentParameterFactory.CascadingValue(_modalService.Object));

            // Act
            var blogToDelete = blogs.First();

            // Assert
            blogPage.Find("td>div>button").Click();
            _blogService.Verify(service => service.Delete(blogToDelete.Id.ToString()), Times.Once);
            var blogsTableBody = blogPage.Find("tbody");
            blogsTableBody.ChildElementCount.Should().Be(9);
        }

    }
}
