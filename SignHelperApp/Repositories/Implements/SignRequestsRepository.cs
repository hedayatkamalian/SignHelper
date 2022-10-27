using Microsoft.EntityFrameworkCore;
using SignHelperApp.Data;
using SignHelperApp.Entities;
using SignHelperApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SignHelperApp.Repositories.Implements
{
    public class SignRequestsRepository : ISignRequestsRepository
    {
        private readonly DataContext _dataContext;

        public SignRequestsRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<SignRequest?> GetAsync(Expression<Func<SignRequest, bool>> predicate)
        {
            return await _dataContext.SignRequests
                .Include(p => p.SignTemplate)
                .FirstOrDefaultAsync(predicate);
        }


        public async Task<SignRequest?> GetAsync(long id)
        {
            return await GetAsync(p => p.Id == id);
        }

        public void Delete(SignRequest template)
        {
            _dataContext.SignRequests.Remove(template);
        }

        public async Task AddAsync(SignRequest signReques)
        {
            await _dataContext.SignRequests.AddAsync(signReques);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dataContext.SaveChangesAsync();
        }
    }
}
