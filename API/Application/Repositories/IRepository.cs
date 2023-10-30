using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MultiTenantOpenProject.API.Entities;

namespace MultiTenantOpenProject.API.Repositories;

/// <summary>
/// The IRepository is used to create an abstraction layer between the data access layer and business logic layer.
/// </summary>
/// <typeparam name="T">The Entity Type that should be used for the base repository actions.</typeparam>
public interface IRepository<T>
    where T : BaseEntity
{
    public DbContext Context { get; set; }

    /// <summary>
    /// Clears tracking changes
    /// </summary>
    void ClearTracker();

    /// <summary>
    /// Returns all entities of the specified Type T.
    /// </summary>
    /// <returns>List of Type T</returns>
    Task<IEnumerable<T>> FindAllAsync();

    /// <summary>
    /// Returns all entities of the specified Type T with added includes.
    /// </summary>
    /// <param name="includesExpression">The include expression</param>
    /// <returns>List of Type T</returns>
    Task<IEnumerable<T>> FindAllIncludeAsync(params Expression<Func<T, object>>[] includesExpression);

    /// <summary>
    /// Returns entities of the specified Type T which match the provided conditions
    /// </summary>
    /// <param name="conditionsExpression">The conditions expression to match against</param>
    /// <returns>List of Type T</returns>
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> conditionsExpression);

    /// <summary>
    /// Returns entities of the specified Type T with added includes which match the provided conditions
    /// </summary>
    /// <param name="conditionsExpression">The conditions expression to match against</param>
    /// <param name="includesExpression">The include expression</param>
    /// <returns></returns>
    Task<IEnumerable<T>> FindByConditionAndIncludeAsync(Expression<Func<T, bool>> conditionsExpression,
        params Expression<Func<T, object>>[] includesExpression);

    /// <summary>
    /// Returns the first entity of specified Type T which matches the provided conditions
    /// </summary>
    /// <param name="conditionsExpression">The conditions expression to match against</param>
    /// <param name="asNoTracking">Defines whether the returned entities should be tracked by EF. Defaults to Not Track (true)</param>
    /// <returns>Type T</returns>
    Task<T?> FirstByConditionAsync(Expression<Func<T, bool>> conditionsExpression, bool asNoTracking = true);

    /// <summary>
    /// Returns the first entity of specified Type T with the added includes which matches the provided conditions
    /// </summary>
    /// <param name="conditionsExpression">The conditions expression to match against</param>
    /// <param name="includesExpression">The include expression</param>
    /// <returns>Type T?</returns>
    Task<T?> FirstByConditionByIncludeAsync(
        Expression<Func<T, bool>> conditionsExpression,
        bool asNoTracking = true,
        params Expression<Func<T, object>>[] includesExpression);

    /// <summary>
    /// Explicit request to load the specified related entities of the current entity Type T
    /// </summary>
    /// <param name="entity">The parent entity of the related entities we wish to load.</param>
    /// <param name="propertyExpression">Expression notating which related entities should be loaded.</param>
    /// <returns>Type T</returns>
    Task<T> ExplicitLoadAsync(T entity, Expression<Func<T, object?>> propertyExpression);

    /// <summary>
    /// Explicit request to load the specified related collection entities of the current entity Type T
    /// </summary>
    /// <param name="entity">The parent entity of the related entities we wish to load.</param>
    /// <param name="propertyExpression">Expression notating which related entities should be loaded.</param>
    /// <returns>Type T</returns>
    Task<T> ExplicitLoadCollectionAsync(T entity, Expression<Func<T, IEnumerable<object>>> propertyExpression);

    /// <summary>
    /// Marks an entity to be added to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entity">The entity to be created.</param>
    /// <returns>Type T</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Marks an entity to be added to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entities">The entities to be created.</param>
    /// <returns>Type T</returns>
    IEnumerable<T> Create(IEnumerable<T> entities);

    /// <summary>
    /// Marks an entity to be updated to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entity">The entity to be updated.</param>
    void Update(T entity);

    /// <summary>
    /// Marks a range of entities to be updated to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entities">The entities to be updated.</param>
    void UpdateRange(T[] entities);

    /// <summary>
    /// Marks an entity to be deleted to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entity">The entity to be deleted.</param>
    /// <returns>True/False</returns>
    bool Delete(T entity);

    /// <summary>
    /// Marks a range of entities to be deleted to the persistence system.
    /// </summary>
    /// <remarks>This function does not actually persist the data.</remarks>
    /// <param name="entities">The entity range to be deleted.</param>
    /// <returns>True/False</returns>
    bool DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Executes a raw sql command. NOT SAFE.
    /// </summary>
    /// <param name="sql">The raw sql to be executed (parameterized)</param>
    /// <param name="parameters">The parameters to pass into the Raw SQL</param>
    /// <returns>List of Type T</returns>
    Task<IEnumerable<T>> RawSqlAsync(string sql, params object[] parameters);

    /// <summary>
    /// Executes a raw interpolated sql command. NOT SAFE.
    /// </summary>
    /// <param name="sql">The raw sql to be executed (with interpolated parameters)</param>
    /// <returns>List of Type T</returns>
    Task<IEnumerable<T>> InterpolatedSqlAsync(FormattableString sql);

    /// <summary>
    /// Saves the changes to the persistence layer.
    /// </summary>
    /// <returns>The number of affected rows</returns>
    Task<int> SaveAsync();

    /// <summary>
    /// Saves the changes to the persistence layer.
    /// </summary>
    int Save();
}
