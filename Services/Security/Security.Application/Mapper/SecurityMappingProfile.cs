using AutoMapper;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.DTO.Response;
using Security.Domain.Entities;

namespace Security.Application.Mapper
{
    public class SecurityMappingProfile : Profile
    {
        public SecurityMappingProfile()
        {
            CreateMap<Permissions, PermissionResponse>().ReverseMap();
            CreateMap<Permissions, ModifyPermissionCommand>().ReverseMap();
            CreateMap<Permissions, RequestPermissionCommand>().ReverseMap();
        }
    }
}
