using SexShop.Domain.Common;
using System.Linq.Expressions;

namespace SexShop.Application.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    }
}
