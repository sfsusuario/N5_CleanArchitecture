using System.Threading.Tasks;

namespace Security.Domain.External.Command.Base
{
    /// <summary>
    /// CommandRepository base generic class
    /// </summary>
    public interface ICommandExternal<T> where T : class
    {
        Task<T> RequestAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
