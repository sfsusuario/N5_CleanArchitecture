using MediatR;
using Security.Domain.Constants;
using Security.Domain.Contracts.Persistence;
using Security.Domain.CQRS.External.Commands;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.Entities;
using Security.Domain.External.Command;
using Security.Domain.Repositories.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Security.Application.Handlers.QueryHandlers
{
    /// <summary>
    /// Get permissions handler
    /// </summary>
    public class GetPermissionsHandler : IRequestHandler<GetPermissionsQuery, List<Permissions>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionsQueryRepository _repo;
        private readonly IKafkaCommandExternal _kafka;

        /// <summary>
        /// GetPermissionsHandler constructor
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance</param>
        /// <param name="kafka">Kafka instance</param>
        public GetPermissionsHandler(IUnitOfWork unitOfWork, IKafkaCommandExternal kafka)
        {
            _unitOfWork = unitOfWork;
            _repo = unitOfWork.PermissionsQueryRepository;
            _kafka = kafka;
        }

        /// <summary>
        /// Handle function
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<List<Permissions>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            await _kafka.RequestAsync(new RequestKafkaCommand()
            {
                Id = Guid.NewGuid(),
                NameOperation = KafkaPermissionActions.GET
            });
            return (List<Permissions>)await _repo.GetPermissionsAsync();
        }
    }
}