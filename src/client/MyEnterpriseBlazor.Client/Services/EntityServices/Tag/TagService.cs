using System.Net.Http;
using AutoMapper;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Tag
{
    public class TagService : AbstractEntityService<TagModel, TagDto>, ITagService
    {
        public TagService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IMapper mapper)
            : base(httpClient, authenticationStateProvider, mapper, "/api/tags")
        {
        }
    }
}
