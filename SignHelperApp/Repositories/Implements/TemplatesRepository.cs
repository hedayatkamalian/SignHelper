using Microsoft.EntityFrameworkCore;
using SignHelperApp.Data;
using SignHelperApp.Entities;
using SignHelperApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SignHelperApp.Repositories.Implements
{
    public class TemplatesRepository : ITemplatesRepository
    {
        private readonly DataContext _dataContext;

        public TemplatesRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Template?> GetAsync(Expression<Func<Template, bool>> predicate)
        {
            return _dataContext.Templates.FirstOrDefault(predicate);
        }

        public async Task<Template?> GetAsync(long id)
        {
            return await GetAsync(p => p.Id == id && !p.Deleted);
        }

        public async Task AddAsync(Template template)
        {
            await _dataContext.Templates.AddAsync(template);
        }

        public void Delete(Template template)
        {
            _dataContext.Templates.Remove(template);
        }

        public async Task<IList<Template>> GetAll()
        {
            return await _dataContext.Templates.Where(p => !p.Deleted).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Template, bool>> predicate)
        {
            return await _dataContext.Templates.AnyAsync(predicate);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dataContext.SaveChangesAsync();
        }


    }
}
