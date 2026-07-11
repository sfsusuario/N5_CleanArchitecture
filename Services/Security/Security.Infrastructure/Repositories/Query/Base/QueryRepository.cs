using Security.Domain.Repositories.Query.Base;
using Security.Infrastructure.Data;

namespace Security.Infrastructure.Repository.Query.Base
{
    /// <summary>
    /// QueryRepository base class
    /// </summary>
    public class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        /// <summary>
        /// Security context instance
        /// </summary>
        protected readonly SecurityContext _context;

        /// <summary>
        ///
        /// </summary>
        /// <param name="context">Security context instance</param>
        public QueryRepository(SecurityContext context)
        {
            _context = context;
        }
    }
}