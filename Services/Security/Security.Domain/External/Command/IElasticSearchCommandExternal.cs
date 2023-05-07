using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command.Base;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.External.Command
{
    public interface IElasticSearchCommandExternal : ISingleCommandRepository<RequestElasticSearchCommand>
    {
    }
}
