using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Extensions
{
    /// <summary>
    /// Extension methods for querying soft-deleted entities.
    /// </summary>
    public static class SoftDeleteQueryExtensions
    {
        /// <summary>
        /// Ignore the global soft delete filter to include inactive (Activo=false) records.
        /// Use only in admin scenarios where viewing all records including deleted ones is required.
        /// </summary>
        public static IQueryable<T> IncludeSoftDeleted<T>(this IQueryable<T> query)
            where T : AuditableEntity
        {
            return query.IgnoreQueryFilters();
        }

        /// <summary>
        /// Query only soft-deleted records (Activo=false).
        /// </summary>
        public static IQueryable<T> OnlySoftDeleted<T>(this IQueryable<T> query)
            where T : AuditableEntity
        {
            return query.IgnoreQueryFilters().Where(e => !e.Activo);
        }

        /// <summary>
        /// Check if an entity is soft-deleted.
        /// </summary>
        public static bool IsSoftDeleted<T>(this T entity)
            where T : AuditableEntity
        {
            return !entity.Activo;
        }
    }
}
