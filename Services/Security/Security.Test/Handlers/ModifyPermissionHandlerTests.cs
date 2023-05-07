using AutoMapper;
using Moq;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.Contracts.Persistence;
using Security.Domain.DTO.Response;
using Security.Application.Handlers.CommandHandler;
using Security.Application.Mapper;
using Security.Domain.Entities;
using Security.Test.Mocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Security.Domain.External.Command;

namespace Security.Test.Handlers
{
    public class ModifyPermissionHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Permissions _leaveTypeDto;
        private readonly ModifyPermissionHandler _handler;
        private readonly Mock<IKafkaCommandExternal> _mockKafka;
        private readonly Mock<IElasticSearchCommandExternal> _mockElasticsearch;

        public ModifyPermissionHandlerTests()
        {
            _mockUow = MockUnitOfWork.GetUnitOfWork();
            _mockKafka = MockKafka.GetKafka();
            _mockElasticsearch = MockElasticSearch.GetElasticSearch();

            var mapperConfig = new MapperConfiguration(c => 
            {
                c.AddProfile<SecurityMappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();
            _handler = new ModifyPermissionHandler(_mockUow.Object, _mockKafka.Object, _mockElasticsearch.Object);

            _leaveTypeDto = new Permissions
            {
                Id = 1,
                EmployeeForename = "Samael",
                EmployeeSurname = "Fierro",
            };
        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            var result = await _handler.Handle(new ModifyPermissionCommand(), CancellationToken.None);

            result.ShouldBeOfType<PermissionResponse>();
        }
    }
}
