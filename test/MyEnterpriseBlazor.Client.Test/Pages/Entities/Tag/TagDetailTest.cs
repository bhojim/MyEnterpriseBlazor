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
using MyEnterpriseBlazor.Client.Pages.Entities.Tag;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Tag;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Tag
{
    public class TagDetailTest : TestContext
    {
        private readonly Mock<ITagService> _tagService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public TagDetailTest()
        {
            _tagService = new Mock<ITagService>();
            _navigationService = new Mock<INavigationService>();
            Services.AddSingleton<ITagService>(_tagService.Object);
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
        public void Should_DisplayTag_When_IdIsPresent()
        {
            //Arrange
            var tag = _fixture.Create<TagModel>();
            _tagService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(tag));
            var tagDetail = RenderComponent<TagDetail>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var title = tagDetail.Find("h2");

            // Assert
            title.MarkupMatches($"<h2><span>Tag</span>{tag.Id}</h2>");

        }

        [Fact]
        public void Should_NotDisplayTag_When_IdIsNotPresent()
        {
            //Arrange
            _tagService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(new TagModel()));
            var tagDetail = RenderComponent<TagDetail>();

            // Act
            var title = tagDetail.Find("div.col-8");

            // Assert
            title.Children.Length.Should().Be(0);

        }
    }
}
