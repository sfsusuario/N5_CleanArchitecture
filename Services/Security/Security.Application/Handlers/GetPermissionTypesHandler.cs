using MediatR;
using Security.Domain.Contracts.Persistence;
using Security.Domain.CQRS.Repository.Queries;
using Security.Domain.Entities;
using Security.Domain.Repositories.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Security.Application.Handlers.QueryHandlers
{
    /// <summary>
    /// Get permission types handler
    /// </summary>
    public class GetPermissionTypesHandler : IRequestHandler<GetPermissionTypesQuery, List<PermissionsType>>
    {
        private readonly IPermissionTypesQueryRepository _repo;

        /// <summary>
        /// GetPermissionTypesHandler constructor
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance</param>
        public GetPermissionTypesHandler(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork.PermissionTypesQueryRepository;
        }

        /// <summary>
        /// Handle function
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<List<PermissionsType>> Handle(GetPermissionTypesQuery request, CancellationToken cancellationToken)
        {
            return (await _repo.GetAllAsync()).ToList();
        }
    }
}
