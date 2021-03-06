using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace MyEnterpriseBlazor.Client.Services.EntityServices.User
{
    public class UserService : AbstractEntityService<UserModel, UserDto>, IUserService
    {
        public UserService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IMapper mapper) : base(httpClient, authenticationStateProvider, mapper, "/api/users")
        {
        }

        public async Task<IEnumerable<string>> GetAllAuthorities()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<string>>($"{BaseUrl}/authorities");
        }
    }
}
