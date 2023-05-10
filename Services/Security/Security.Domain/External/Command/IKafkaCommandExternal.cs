using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command.Base;

namespace Security.Domain.External.Command
{
    /// <summary>
    /// Interface kafka command external
    /// </summary>
    public interface IKafkaCommandExternal: ISingleCommandRepository<RequestKafkaCommand>
    {
    }
}
