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
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Tag
{
    public class TagTest : TestContext
    {
        private readonly Mock<ITagService> _tagService;
        private readonly Mock<IModalService> _modalService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public TagTest()
        {
            _tagService = new Mock<ITagService>();
            _modalService = new Mock<IModalService>();
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
        public void Should_DisplayAllTags_When_TagsArePresent()
        {
            //Arrange
            var tags = _fixture.CreateMany<TagModel>(10);
            _tagService.Setup(service => service.GetAll()).Returns(Task.FromResult(tags.ToList() as IList<TagModel>));
            var tagPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Tag.Tag>();

            // Act
            var tagsTableBody = tagPage.Find("tbody");

            // Assert
            tagsTableBody.ChildElementCount.Should().Be(10);
        }

        [Fact]
        public void Should_DisplayNoCountry_When_TagsLengthIsZero()
        {
            //Arrange
            var tags = new List<TagModel>();
            _tagService.Setup(service => service.GetAll()).Returns(Task.FromResult(tags.ToList() as IList<TagModel>));
            var tagPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Tag.Tag>();

            // Act
            var span = tagPage.Find("div>span");

            // Assert
            span.MarkupMatches("<span>No Tags found</span>");
        }

        [Fact]
        public void Should_DeleteTag_WhenDeleteButtonClicked()
        {
            //Arrange
            var tags = _fixture.CreateMany<TagModel>(10);
            _tagService.Setup(service => service.GetAll()).Returns(Task.FromResult(tags.ToList() as IList<TagModel>));

            var modalRef = new Mock<IModalReference>();
            modalRef.Setup(mock => mock.Result).Returns(Task.FromResult(ModalResult.Ok(new { })));
            _modalService.Setup(service => service.Show<DeleteModal>(It.IsAny<string>())).Returns(modalRef.Object);
            var tagPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Tag.Tag>(ComponentParameterFactory.CascadingValue(_modalService.Object));

            // Act
            var tagToDelete = tags.First();

            // Assert
            tagPage.Find("td>div>button").Click();
            _tagService.Verify(service => service.Delete(tagToDelete.Id.ToString()), Times.Once);
            var tagsTableBody = tagPage.Find("tbody");
            tagsTableBody.ChildElementCount.Should().Be(9);
        }

    }
}
