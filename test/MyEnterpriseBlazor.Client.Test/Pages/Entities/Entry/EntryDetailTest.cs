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
using MyEnterpriseBlazor.Client.Pages.Entities.Entry;
using MyEnterpriseBlazor.Client.Pages.Utils;
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Entry
{
    public class EntryDetailTest : TestContext
    {
        private readonly Mock<IEntryService> _entryService;
        private readonly Mock<INavigationService> _navigationService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public EntryDetailTest()
        {
            _entryService = new Mock<IEntryService>();
            _navigationService = new Mock<INavigationService>();
            Services.AddSingleton<IEntryService>(_entryService.Object);
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
        public void Should_DisplayEntry_When_IdIsPresent()
        {
            //Arrange
            var entry = _fixture.Create<EntryModel>();
            _entryService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(entry));
            var entryDetail = RenderComponent<EntryDetail>(ComponentParameter.CreateParameter("Id", 1));

            // Act
            var title = entryDetail.Find("h2");

            // Assert
            title.MarkupMatches($"<h2><span>Entry</span>{entry.Id}</h2>");

        }

        [Fact]
        public void Should_NotDisplayEntry_When_IdIsNotPresent()
        {
            //Arrange
            _entryService.Setup(service => service.Get(It.IsAny<string>())).Returns(Task.FromResult(new EntryModel()));
            var entryDetail = RenderComponent<EntryDetail>();

            // Act
            var title = entryDetail.Find("div.col-8");

            // Assert
            title.Children.Length.Should().Be(0);

        }
    }
}
