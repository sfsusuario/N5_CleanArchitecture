using MediatR;
using Security.Domain.Contracts.Persistence;
using Security.Domain.DTO.Response;
using Security.Application.Mapper;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using System;
using System.Threading;
using System.Threading.Tasks;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.External.Command;
using Security.Domain.Constantnts;
using Security.Domain.CQRS.External.Commands;

namespace Security.Application.Handlers.CommandHandler
{
    public class ModifyPermissionHandler : IRequestHandler<ModifyPermissionCommand, PermissionResponse>
    {
        private readonly IPermissionsQueryRepository _repoQuery;
        private readonly IPermissionsCommandRepository _repoCommand;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaCommandExternal _kafka;

        public ModifyPermissionHandler(IUnitOfWork unitOfWork, IKafkaCommandExternal kafka)
        {
            _unitOfWork = unitOfWork;
            _repoQuery = _unitOfWork.PermissionsQueryRepository;
            _repoCommand = _unitOfWork.PermissionsCommandRepository;
            _kafka = kafka;
        }
        
        public async Task<PermissionResponse> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionsEntity = PermissionsMapper.Mapper.Map<Permissions>(request);

            if (permissionsEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                await _repoCommand.UpdateAsync(permissionsEntity);
                await _kafka.RequestAsync(new RequestKafkaCommand()
                {
                    Id = Guid.NewGuid(),
                    NameOperation = KafkaPermissionActions.MODIFY
                });
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedPermissions = await _repoQuery.GetPermissionAsync(request.Id);
            var permissionsResponse = PermissionsMapper.Mapper.Map<PermissionResponse>(modifiedPermissions);

            return permissionsResponse;
        }
    }
}