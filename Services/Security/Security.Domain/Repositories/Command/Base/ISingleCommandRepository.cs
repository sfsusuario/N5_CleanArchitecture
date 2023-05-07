using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.Repositories.Command.Base
{
    /// <summary>
    /// ISingleRepository base generic class
    /// </summary>
    public interface ISingleCommandRepository<T> where T : class
    {
        Task<T> RequestAsync(T entity);
    }
}
