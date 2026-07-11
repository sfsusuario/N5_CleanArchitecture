using AutoMapper;
using Security.Application.Mapper;
using Xunit;

namespace Security.Test.Mapper
{
    public class SecurityMappingProfileTests
    {
        [Fact]
        public void Configuration_IsValid()
        {
            var configuration = new MapperConfiguration(c => c.AddProfile<SecurityMappingProfile>());

            configuration.AssertConfigurationIsValid();
        }
    }
}
