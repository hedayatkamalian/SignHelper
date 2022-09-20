using SignHelperApp.Entities;
using System.Linq.Expressions;

namespace SignHelperApp.Repositories.Interfaces
{
    public interface ISignRequestsRepository
    {
        Task<SignRequest?> GetAsync(Expression<Func<SignRequest, bool>> predicate);
        Task<SignRequest?> GetAsync(long id);
        void Delete(SignRequest template);
        Task AddAsync(SignRequest signReques);
        Task<int> SaveChangesAsync();
    }
}
