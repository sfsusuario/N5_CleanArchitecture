using Security.Domain.Entities;
using Security.Domain.External.Command.Base;

namespace Security.Domain.Repositories.Command
{
    /// <summary>
    /// PermissionCommandRepository interface
    /// </summary>
    public interface IPermissionsCommandRepository : ICommandExternal<Permissions>
    {

    }
}