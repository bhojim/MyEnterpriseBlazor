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
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using MyEnterpriseBlazor.Client.Services.EntityServices.Blog;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Entry
{
    public class EntryUpdateTest : TestContext
    {
        private readonly Mock<IEntryService> _entryService;
        private readonly Mock<IBlogService> _blogService;
        private readonly Mock<ITagService> _tagService;
        private readonly Mock<IModalService> _modalService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public EntryUpdateTest()
        {
            _entryService = new Mock<IEntryService>();
            _modalService = new Mock<IModalService>();
            _navigationService = new Mock<INavigationService>();
            _blogService = new Mock<IBlogService>();
            Services.AddSingleton<IBlogService>(_blogService.Object);
            _tagService = new Mock<ITagService>();
            Services.AddSingleton<ITagService>(_tagService.Object);
            Services.AddSingleton<INavigationService>(_navigationService.Object);
            Services.AddSingleton<IEntryService>(_entryService.Object);
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
        public void Should_UpdateSelectedEntry_When_FormIsSubmitWithId()
        {
            //Arrange
            var blogs = _fixture.CreateMany<BlogModel>(10);
            _blogService.Setup(service => service.GetAll()).Returns(Task.FromResult(blogs.ToList() as IList<BlogModel>));
            var tags = _fixture.CreateMany<TagModel>(10);
            _tagService.Setup(service => service.GetAll()).Returns(Task.FromResult(tags.ToList() as IList<TagModel>));

            var entryToUpdate = _fixture.Create<EntryModel>();
            entryToUpdate.Id = 1;

            _entryService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(entryToUpdate));

            var updateEntryPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Entry.EntryUpdate>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var entryForm = updateEntryPage.Find("form");
            entryForm.Submit();

            // Assert
            _entryService.Verify(service => service.Update(entryToUpdate), Times.Once);
        }

        [Fact]
        public void Should_AddEntry_When_FormIsSubmitWithoutId()
        {
            //Arrange
            var blogs = _fixture.CreateMany<BlogModel>(10);
            _blogService.Setup(service => service.GetAll()).Returns(Task.FromResult(blogs.ToList() as IList<BlogModel>));
            var tags = _fixture.CreateMany<TagModel>(10);
            _tagService.Setup(service => service.GetAll()).Returns(Task.FromResult(tags.ToList() as IList<TagModel>));

            var updateEntryPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Entry.EntryUpdate>(ComponentParameter.CreateParameter("Id", 0));

            // Act
            var entryForm = updateEntryPage.Find("form");
            entryForm.Submit();

            // Assert
            _entryService.Verify(service => service.Add(It.IsAny<EntryModel>()), Times.Once);
        }

    }
}
