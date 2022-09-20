using SignHelperApp.Entities;
using System.Linq.Expressions;

namespace SignHelperApp.Repositories.Interfaces
{
    public interface ITemplatesRepository
    {
        Task<Template?> GetAsync(Expression<Func<Template, bool>> predicate);
        Task<Template?> GetAsync(long id);
        Task<IList<Template>> GetAll();
        void Delete(Template template);
        Task AddAsync(Template template);
        Task<bool> AnyAsync(Expression<Func<Template, bool>> predicate);
        Task<int> SaveChangesAsync();


    }
}