using MediatR;
using Security.Domain.Contracts.Persistence;
using Security.Domain.CQRS.Repository.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command;
using System.Threading;
using System.Threading.Tasks;

namespace Security.Application.Handlers.CommandHandler
{
    /// <summary>
    /// Create permission type handler
    /// </summary>
    public class CreatePermissionTypeHandler : IRequestHandler<CreatePermissionTypeCommand, PermissionsType>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionTypesCommandRepository _repo;

        /// <summary>
        /// CreatePermissionTypeHandler constructor
        /// </summary>
        /// <param name="unitOfWork">UnitOfWork instance</param>
        public CreatePermissionTypeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _repo = unitOfWork.PermissionTypesCommandRepository;
        }

        /// <summary>
        /// Handle function
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public async Task<PermissionsType> Handle(CreatePermissionTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new PermissionsType { Description = request.Description };

            var created = await _repo.RequestAsync(entity);
            await _unitOfWork.Save();

            return created;
        }
    }
}
