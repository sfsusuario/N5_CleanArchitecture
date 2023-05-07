using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command.Base;

namespace Security.Domain.External.Command
{
    public interface IKafkaCommandExternal: ISingleCommandRepository<RequestKafkaCommand>
    {
    }
}
