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
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Tag
{
    public class TagUpdateTest : TestContext
    {
        private readonly Mock<ITagService> _tagService;
        private readonly Mock<IModalService> _modalService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public TagUpdateTest()
        {
            _tagService = new Mock<ITagService>();
            _modalService = new Mock<IModalService>();
            _navigationService = new Mock<INavigationService>();
            Services.AddSingleton<INavigationService>(_navigationService.Object);
            Services.AddSingleton<ITagService>(_tagService.Object);
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
        public void Should_UpdateSelectedTag_When_FormIsSubmitWithId()
        {
            //Arrange

            var tagToUpdate = _fixture.Create<TagModel>();
            tagToUpdate.Id = 1;

            _tagService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(tagToUpdate));

            var updateTagPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Tag.TagUpdate>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var tagForm = updateTagPage.Find("form");
            tagForm.Submit();

            // Assert
            _tagService.Verify(service => service.Update(tagToUpdate), Times.Once);
        }

        [Fact]
        public void Should_AddTag_When_FormIsSubmitWithoutId()
        {
            //Arrange

            var updateTagPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Tag.TagUpdate>(ComponentParameter.CreateParameter("Id", 0));

            // Act
            var tagForm = updateTagPage.Find("form");
            tagForm.Submit();

            // Assert
            _tagService.Verify(service => service.Add(It.IsAny<TagModel>()), Times.Once);
        }

    }
}
