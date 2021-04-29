using AutoMapper;
using System.Linq;
using MyEnterpriseBlazor.Client.Models;
using MyEnterpriseBlazor.Dto;


namespace MyEnterpriseBlazor.Client.AutoMapper
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, UserModel>()
                .ForMember(userModel => userModel.Authorities,
                    opt => opt.MapFrom(userDto => userDto.Roles))
                .ReverseMap()
                .ForPath(userDto => userDto.Roles,
                    opt => opt.MapFrom(userModel => userModel.Authorities));

            CreateMap<ManagedUserDto, UserSaveModel>().ReverseMap();
            CreateMap<LoginDto, LoginModel>().ReverseMap();

            CreateMap<BlogModel, BlogDto>().ReverseMap();
            CreateMap<EntryModel, EntryDto>().ReverseMap();
            CreateMap<TagModel, TagDto>().ReverseMap();
            // jhipster-needle-add-dto-model-mapping - JHipster will add dto to model and model to dto mapping
        }
    }
}
