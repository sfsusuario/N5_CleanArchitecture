using System.Threading.Tasks;

namespace Security.Domain.External.Command.Base
{
    /// <summary>
    /// CommandRepository base generic class
    /// </summary>
    public interface ICommandExternal<T> where T : class
    {
        /// <summary>
        /// Request async
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Type</returns>
        Task<T> RequestAsync(T entity);

        /// <summary>
        /// Update async
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Type</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Delete async
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Type</returns>
        Task DeleteAsync(T entity);
    }
}
