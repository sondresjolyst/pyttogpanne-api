using Microsoft.EntityFrameworkCore;

namespace pyttogpanne_api.Infrastructure
{
    /// <summary>
    /// Runs a split query again once when EF reports that rows changed between its SQL statements.
    /// A second failure goes to the exception handler, which answers 503.
    /// </summary>
    public static class SplitQueryRetry
    {
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> query)
        {
            try
            {
                return await query();
            }
            catch (DbQueryConcurrencyException)
            {
                return await query();
            }
        }
    }
}
