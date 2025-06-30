using Microsoft.EntityFrameworkCore;
using Base.Interfaces;
using Base.Models;
using Base.Specifications;

namespace Base.Repositories;

public class GenericRepository<TDbContext, TModel> : IRepository<TModel> where TModel : BaseModel where TDbContext : DbContext
{
    protected readonly TDbContext _context;
    protected readonly DbSet<TModel> _dbSet;
    public GenericRepository(TDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TModel>();
    }

    public async virtual Task<IEnumerable<TModel>> GetAllAsync() =>
        await _dbSet.ToListAsync();
    public async Task<IEnumerable<TModel>> GetAllAsync(Specification<TModel> specification) =>
        await SpecificationQueryBuilder.Build(_dbSet, specification).ToListAsync();

    public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Specification<TModel, TResult> specification) =>
        await SpecificationQueryBuilder.Build(_dbSet, specification).ToListAsync();

    public async Task<TModel?> GetOneAsync(Specification<TModel> specification) =>
        await SpecificationQueryBuilder.Build(_dbSet, specification).FirstOrDefaultAsync();

    public async Task<TResult?> GetOneAsync<TResult>(Specification<TModel, TResult> specification) =>
        await SpecificationQueryBuilder.Build(_dbSet, specification).FirstOrDefaultAsync();

    public async virtual Task<TModel?> GetByIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    public async virtual Task<TModel> AddAsync(TModel model) =>
        (await _dbSet.AddAsync(model)).Entity;

    public async virtual Task AddRangeAsync(IEnumerable<TModel> models) =>
        await _dbSet.AddRangeAsync(models);

    public virtual TModel Update(TModel model) =>
        _dbSet.Update(model).Entity;

    public virtual void Delete(TModel model) =>
        _dbSet.Remove(model);

    public virtual void DeleteRange(IEnumerable<TModel> models) =>
        _dbSet.RemoveRange(models);

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}
