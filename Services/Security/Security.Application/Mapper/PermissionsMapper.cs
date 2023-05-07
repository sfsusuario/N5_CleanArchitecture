using AutoMapper;
using System;

namespace Security.Application.Mapper
{
    /// <summary>
    /// Profile class for mapping commands to entities
    /// </summary>
    public class PermissionsMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(()=>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<SecurityMappingProfile>();
            });

            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }
}