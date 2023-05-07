
using MediatR;
using Security.Application.Mapper;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using Security.Domain.Contracts.Persistence;
using Security.Domain.DTO.Response;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.External.Command;
using Security.Domain.Constantnts;
using Security.Domain.CQRS.External.Commands;

namespace Security.Application.Handlers.CommandHandler
{
    public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, PermissionResponse>
    {
        private readonly IPermissionsQueryRepository _repoQuery;
        private readonly IPermissionsCommandRepository _repoCommand;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKafkaCommandExternal _kafka;

        public RequestPermissionHandler(IUnitOfWork unitOfWork, IKafkaCommandExternal kafka)
        {
            _unitOfWork = unitOfWork;
            _repoQuery = _unitOfWork.PermissionsQueryRepository;
            _repoCommand = _unitOfWork.PermissionsCommandRepository;
            _kafka = kafka;
        }

        public async Task<PermissionResponse> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionsEntity = PermissionsMapper.Mapper.Map<Permissions>(request);
            Permissions createdPermission = null;

            if (permissionsEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            try
            {
                createdPermission = await _repoCommand.RequestAsync(permissionsEntity);
                await _kafka.RequestAsync(new RequestKafkaCommand()
                {
                    Id = Guid.NewGuid(),
                    NameOperation = KafkaPermissionActions.REQUEST
                });
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var permissionsResponse = PermissionsMapper.Mapper.Map<PermissionResponse>(createdPermission);
            return permissionsResponse;
        }
    }
}