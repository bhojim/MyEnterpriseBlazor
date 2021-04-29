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
using MyEnterpriseBlazor.Client.Services.EntityServices.Entry;
using MyEnterpriseBlazor.Client.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Xunit;

namespace MyEnterpriseBlazor.Client.Test.Pages.Entities.Entry
{
    public class EntryTest : TestContext
    {
        private readonly Mock<IEntryService> _entryService;
        private readonly Mock<IModalService> _modalService;
        private readonly AutoFixture.Fixture _fixture = new AutoFixture.Fixture();

        public EntryTest()
        {
            _entryService = new Mock<IEntryService>();
            _modalService = new Mock<IModalService>();
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
        public void Should_DisplayAllEntries_When_EntriesArePresent()
        {
            //Arrange
            var entries = _fixture.CreateMany<EntryModel>(10);
            _entryService.Setup(service => service.GetAll()).Returns(Task.FromResult(entries.ToList() as IList<EntryModel>));
            var entryPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Entry.Entry>();

            // Act
            var entriesTableBody = entryPage.Find("tbody");

            // Assert
            entriesTableBody.ChildElementCount.Should().Be(10);
        }

        [Fact]
        public void Should_DisplayNoCountry_When_EntriesLengthIsZero()
        {
            //Arrange
            var entries = new List<EntryModel>();
            _entryService.Setup(service => service.GetAll()).Returns(Task.FromResult(entries.ToList() as IList<EntryModel>));
            var entryPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Entry.Entry>();

            // Act
            var span = entryPage.Find("div>span");

            // Assert
            span.MarkupMatches("<span>No Entries found</span>");
        }

        [Fact]
        public void Should_DeleteEntry_WhenDeleteButtonClicked()
        {
            //Arrange
            var entries = _fixture.CreateMany<EntryModel>(10);
            _entryService.Setup(service => service.GetAll()).Returns(Task.FromResult(entries.ToList() as IList<EntryModel>));

            var modalRef = new Mock<IModalReference>();
            modalRef.Setup(mock => mock.Result).Returns(Task.FromResult(ModalResult.Ok(new { })));
            _modalService.Setup(service => service.Show<DeleteModal>(It.IsAny<string>())).Returns(modalRef.Object);
            var entryPage = RenderComponent<MyEnterpriseBlazor.Client.Pages.Entities.Entry.Entry>(ComponentParameterFactory.CascadingValue(_modalService.Object));

            // Act
            var entryToDelete = entries.First();

            // Assert
            entryPage.Find("td>div>button").Click();
            _entryService.Verify(service => service.Delete(entryToDelete.Id.ToString()), Times.Once);
            var entriesTableBody = entryPage.Find("tbody");
            entriesTableBody.ChildElementCount.Should().Be(9);
        }

    }
}
