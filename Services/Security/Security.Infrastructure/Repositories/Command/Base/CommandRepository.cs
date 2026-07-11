using Microsoft.EntityFrameworkCore;
using Security.Domain.External.Command.Base;
using Security.Infrastructure.Data;
using System.Threading.Tasks;

namespace Security.Infrastructure.Repository.Command.Base
{
    /// <summary>
    /// CommandRepository base generic class
    /// </summary>
    public class CommandRepository<T> : ICommandExternal<T> where T : class
    {
        protected readonly SecurityContext _context;

        /// <summary>
        /// Repositort constructor
        /// </summary>
        /// <param name="context"></param>
        public CommandRepository(SecurityContext context)
        {
            _context = context;
        }

        // These methods only stage changes on the tracked DbContext; they deliberately do NOT
        // call SaveChangesAsync. Persisting is IUnitOfWork.Save()'s job, so a handler that needs
        // to touch more than one repository can still commit everything in one transaction.

        /// <summary>
        /// Request async
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public async Task<T> RequestAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Update async
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete async
        /// </summary>
        /// <param name="entity">Entity object</param>
        /// <returns>Task</returns>
        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
    }
}