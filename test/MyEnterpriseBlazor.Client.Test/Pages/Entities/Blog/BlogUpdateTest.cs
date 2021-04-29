using System;
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
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Services.EntityServices.User;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Blog
{
    public class BlogUpdateTest : TestContext
    {
        private readonly Mock<IBlogService> _blogService;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IModalService> _modalService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public BlogUpdateTest()
        {
            _blogService = new Mock<IBlogService>();
            _modalService = new Mock<IModalService>();
            _navigationService = new Mock<INavigationService>();
            _userService = new Mock<IUserService>();
            Services.AddSingleton<IUserService>(_userService.Object);
            Services.AddSingleton<INavigationService>(_navigationService.Object);
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
        public void Should_UpdateSelectedBlog_When_FormIsSubmitWithId()
        {
            //Arrange
            var users = _fixture.CreateMany<UserModel>(10);
            _userService.Setup(service => service.GetAll()).Returns(Task.FromResult(users.ToList() as IList<UserModel>));

            var blogToUpdate = _fixture.Create<BlogModel>();
            blogToUpdate.Id = 1;

            _blogService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(blogToUpdate));

            var updateBlogPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Blog.BlogUpdate>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var blogForm = updateBlogPage.Find("form");
            blogForm.Submit();

            // Assert
            _blogService.Verify(service => service.Update(blogToUpdate), Times.Once);
        }

        [Fact]
        public void Should_AddBlog_When_FormIsSubmitWithoutId()
        {
            //Arrange
            var users = _fixture.CreateMany<UserModel>(10);
            _userService.Setup(service => service.GetAll()).Returns(Task.FromResult(users.ToList() as IList<UserModel>));

            var updateBlogPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Blog.BlogUpdate>(ComponentParameter.CreateParameter("Id", 0));

            // Act
            var blogForm = updateBlogPage.Find("form");
            blogForm.Submit();

            // Assert
            _blogService.Verify(service => service.Add(It.IsAny<BlogModel>()), Times.Once);
        }

    }
}
