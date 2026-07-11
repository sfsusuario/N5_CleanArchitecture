using AutoMapper;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.DTO.Response;
using Security.Domain.Entities;

namespace Security.Application.Mapper
{
    /// <summary>
    /// Security mapping profile configuration
    /// </summary>
    public class SecurityMappingProfile : Profile
    {
        /// <summary>
        /// Setup configuration
        /// </summary>
        public SecurityMappingProfile()
        {
            // Permissions.PermissionTypeRef and PermissionResponse.PermissionsType hold the same
            // data under different names, so AutoMapper's by-name convention can't match them.
            CreateMap<Permissions, PermissionResponse>()
                .ForMember(dest => dest.PermissionsType, opt => opt.MapFrom(src => src.PermissionTypeRef))
                .ReverseMap()
                .ForMember(dest => dest.PermissionTypeRef, opt => opt.MapFrom(src => src.PermissionsType));
            CreateMap<Permissions, ModifyPermissionCommand>().ReverseMap();
            CreateMap<Permissions, RequestPermissionCommand>().ReverseMap();
        }
    }
}
