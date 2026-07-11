using MediatR;
using Security.Domain.Contracts.Persistence;
using Security.Domain.DTO.Response;
using Security.Application.Mapper;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.Constants;
using Security.Domain.CQRS.External.Commands;
using System.Text.Json;

namespace Security.Application.Handlers.CommandHandler
{
    /// <summary>
    /// Modify permission handler
    /// </summary>
    public class ModifyPermissionHandler : IRequestHandler<ModifyPermissionCommand, PermissionResponse>
    {
        private readonly IPermissionsQueryRepository _repoQuery;
        private readonly IPermissionsCommandRepository _repoCommand;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// ModifyPermissionHandler constructor
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance</param>
        public ModifyPermissionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repoQuery = _unitOfWork.PermissionsQueryRepository;
            _repoCommand = _unitOfWork.PermissionsCommandRepository;
        }

        /// <summary>
        /// Handle function
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<PermissionResponse> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionsEntity = PermissionsMapper.Mapper.Map<Permissions>(request);

            if (permissionsEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _repoCommand.UpdateAsync(permissionsEntity);

                // Unlike RequestPermissionHandler, the Id is already known (it comes from the
                // command), so the Permissions update and both outbox rows can commit in a
                // single Save() call — still wrapped in a transaction for symmetry/safety.
                await _unitOfWork.OutboxMessages.RequestAsync(new OutboxMessage
                {
                    Channel = OutboxChannels.Kafka,
                    CreatedAt = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(new RequestKafkaCommand
                    {
                        Id = Guid.NewGuid(),
                        NameOperation = KafkaPermissionActions.MODIFY
                    })
                });
                await _unitOfWork.OutboxMessages.RequestAsync(new OutboxMessage
                {
                    Channel = OutboxChannels.Elasticsearch,
                    CreatedAt = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(new RequestElasticSearchCommand
                    {
                        Id = permissionsEntity.Id,
                        EmployeeForename = permissionsEntity.EmployeeForename,
                        EmployeeSurname = permissionsEntity.EmployeeSurname,
                        PermissionDate = permissionsEntity.PermissionDate,
                        PermissionType = permissionsEntity.PermissionType
                    })
                });

                await _unitOfWork.Save();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception exp)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApplicationException(exp.Message);
            }

            var modifiedPermissions = await _repoQuery.GetPermissionAsync(request.Id);
            var permissionsResponse = PermissionsMapper.Mapper.Map<PermissionResponse>(modifiedPermissions);

            return permissionsResponse;
        }
    }
}
