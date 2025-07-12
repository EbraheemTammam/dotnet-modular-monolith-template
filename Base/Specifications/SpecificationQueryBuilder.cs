using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using Base.Models;

namespace Base.Specifications;

public static class SpecificationQueryBuilder
{
    public static IQueryable<TModel> Build<TModel>(IQueryable<TModel> queryable, Specification<TModel> specification)
    where TModel : BaseModel
    {
        if(specification.Criteria is not null)
            queryable = queryable.Where(specification.Criteria);

        if(specification.Includes is not null)
            foreach(Expression<Func<TModel, object>> include in specification.Includes)
                queryable = queryable.Include(include);

        queryable = specification.IncludeStrings.Aggregate(queryable, (current, next) => current.Include(next));

        if(specification.OrderBy is not null)
            queryable = queryable.OrderBy(specification.OrderBy);

        if(specification.OrderByDescending is not null)
            queryable = queryable.OrderByDescending(specification.OrderByDescending);

        return queryable;
    }

    public static IQueryable<TResult> Build<TModel, TResult>(IQueryable<TModel> queryable, Specification<TModel, TResult> specification)
    where TModel : BaseModel =>
        Build<TModel>(queryable, specification).Select(specification.Selector!);
}
