
using MediatR;
using Security.Application.Mapper;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using Security.Domain.Repositories.Query;
using Security.Domain.Contracts.Persistence;
using Security.Domain.DTO.Response;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.Constants;
using Security.Domain.CQRS.External.Commands;
using System.Text.Json;

namespace Security.Application.Handlers.CommandHandler
{
    /// <summary>
    /// Request permission handler
    /// </summary>
    public class RequestPermissionHandler : IRequestHandler<RequestPermissionCommand, PermissionResponse>
    {
        private readonly IPermissionsQueryRepository _repoQuery;
        private readonly IPermissionsCommandRepository _repoCommand;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// RequestPermissionHandler constructor
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance</param>
        public RequestPermissionHandler(IUnitOfWork unitOfWork)
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
        public async Task<PermissionResponse> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionsEntity = PermissionsMapper.Mapper.Map<Permissions>(request);
            Permissions createdPermission = null;

            if (permissionsEntity is null)
            {
                throw new ApplicationException("There is a problem in mapper");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                createdPermission = await _repoCommand.RequestAsync(permissionsEntity);
                // Flush now (not just at the end): createdPermission.Id is SQL Server-generated
                // and only known after this Save() — the Elasticsearch outbox payload needs it.
                // Both this write and the outbox rows below still commit atomically together,
                // because everything happens inside the transaction started above.
                await _unitOfWork.Save();

                await _unitOfWork.OutboxMessages.RequestAsync(new OutboxMessage
                {
                    Channel = OutboxChannels.Kafka,
                    CreatedAt = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(new RequestKafkaCommand
                    {
                        Id = Guid.NewGuid(),
                        NameOperation = KafkaPermissionActions.REQUEST
                    })
                });
                await _unitOfWork.OutboxMessages.RequestAsync(new OutboxMessage
                {
                    Channel = OutboxChannels.Elasticsearch,
                    CreatedAt = DateTime.UtcNow,
                    Payload = JsonSerializer.Serialize(new RequestElasticSearchCommand
                    {
                        Id = createdPermission.Id,
                        EmployeeForename = createdPermission.EmployeeForename,
                        EmployeeSurname = createdPermission.EmployeeSurname,
                        PermissionDate = createdPermission.PermissionDate,
                        PermissionType = createdPermission.PermissionType
                    })
                });
                await _unitOfWork.Save();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception exp)
            {
                // Rolls back the Permissions insert too: with the Outbox pattern, either the
                // business change AND its notifications land together, or neither does.
                // Kafka/Elasticsearch themselves are never called from this request path
                // anymore — see OutboxDispatcherService — so this only ever catches DB errors.
                await _unitOfWork.RollbackTransactionAsync();
                throw new ApplicationException(exp.Message);
            }

            var permissionsResponse = PermissionsMapper.Mapper.Map<PermissionResponse>(createdPermission);
            return permissionsResponse;
        }
    }
}
