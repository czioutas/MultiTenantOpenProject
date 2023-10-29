using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MultiTenantOpenProject.API.Entities;

namespace MultiTenantOpenProject.API.Repositories;
///<inheritdoc cref="IRepository{T}"/>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    public DbContext Context { get; set; }

    public Repository(DbContext context)
    {
        Context = context;
    }

    public void ClearTracker()
    {
        Context.ChangeTracker.Clear();
    }

    public virtual async Task<IEnumerable<T>> FindAllAsync()
    {
        IQueryable<T> query = Context.Set<T>().AsNoTracking();

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAllIncludeAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>().AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = Context.Set<T>().AsNoTracking().Where(expression);

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindByConditionAndIncludeAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>().AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.Where(expression).ToListAsync();
    }

    public virtual async Task<T?> FirstByConditionAsync(Expression<Func<T, bool>> expression, bool asNoTracking = true)
    {
        IQueryable<T> query = Context.Set<T>().Where(expression);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync();
    }

    public Task<T?> FirstByConditionByIncludeAsync(
        Expression<Func<T, bool>> expression,
        bool asNoTracking = true,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = Context.Set<T>().Where(expression);

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query.Where(expression).FirstOrDefaultAsync();
    }

    public virtual int Save()
    {
        return Context.SaveChanges();
    }

    public virtual async Task<int> SaveAsync()
    {
        return await Context.SaveChangesAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await Context.AddAsync(entity);
        return entity;
    }

    public virtual IEnumerable<T> Create(IEnumerable<T> entities)
    {
        Context.AddRange(entities);
        return entities;
    }

    public virtual void Update(T entity)
    {
        Context.Update(entity);
    }

    public virtual void UpdateRange(T[] entities)
    {
        Context.UpdateRange(entities);
    }

    public virtual bool Delete(T entity)
    {
        Context.Remove(entity);
        return true;
    }

    public virtual bool DeleteRange(IEnumerable<T> entities)
    {
        Context.RemoveRange(entities);
        return true;
    }

    public async Task<IEnumerable<T>> RawSqlAsync(string sql, params object[] parameters)
    {
        var a = await Context.Set<T>()
                    .FromSqlRaw(
                        sql,
                        parameters
                    )
                    .ToListAsync();

        return a;
    }

    public async Task<IEnumerable<T>> InterpolatedSqlAsync(FormattableString sql)
    {
        var a = await Context.Set<T>()
                    .FromSqlInterpolated(sql)
                    .ToListAsync();

        return a;
    }

    public async Task<T> ExplicitLoadAsync(T entity, Expression<Func<T, object?>> propertyExpression)
    {
        await Context.Entry(entity).Reference(propertyExpression).LoadAsync();
        return entity;
    }

    public async Task<T> ExplicitLoadCollectionAsync(T entity, Expression<Func<T, IEnumerable<object>>> propertyExpression)
    {
        await Context.Entry(entity).Collection(propertyExpression).LoadAsync();
        return entity;
    }
}