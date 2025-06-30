using Base.Models;
using Base.Specifications;

namespace Base.Interfaces;

public interface IRepository<TModel> where TModel : BaseModel
{
    Task<IEnumerable<TModel>> GetAllAsync();
    Task<IEnumerable<TModel>> GetAllAsync(Specification<TModel> specification);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(Specification<TModel, TResult> specification);
    Task<TModel?> GetOneAsync(Specification<TModel> specification);
    Task<TResult?> GetOneAsync<TResult>(Specification<TModel, TResult> specification);
    Task<TModel?> GetByIdAsync(Guid id);
    Task<TModel> AddAsync(TModel model);
    Task AddRangeAsync(IEnumerable<TModel> models);
    TModel Update(TModel model);
    void Delete(TModel model);
    void DeleteRange(IEnumerable<TModel> models);
    Task SaveAsync();
}
