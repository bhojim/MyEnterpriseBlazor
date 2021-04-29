using System.Net.Http;
using AutoMapper;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.Blog
{
    public class BlogService : AbstractEntityService<BlogModel, BlogDto>, IBlogService
    {
        public BlogService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IMapper mapper)
            : base(httpClient, authenticationStateProvider, mapper, "/api/blogs")
        {
        }
    }
}
