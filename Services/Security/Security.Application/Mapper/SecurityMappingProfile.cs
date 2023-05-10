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
            CreateMap<Permissions, PermissionResponse>().ReverseMap();
            CreateMap<Permissions, ModifyPermissionCommand>().ReverseMap();
            CreateMap<Permissions, RequestPermissionCommand>().ReverseMap();
        }
    }
}
