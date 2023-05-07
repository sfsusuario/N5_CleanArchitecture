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

        public CommandRepository(SecurityContext context)
        {
            _context = context;
        }
        public async Task<T> RequestAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}