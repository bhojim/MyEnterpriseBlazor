using System.Net.Http;
using AutoMapper;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Entry
{
    public class EntryService : AbstractEntityService<EntryModel, EntryDto>, IEntryService
    {
        public EntryService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IMapper mapper)
            : base(httpClient, authenticationStateProvider, mapper, "/api/entries")
        {
        }
    }
}
