using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Models.Interfaces;
using Ir.ApiTest.Entity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Ir.ApiTest.Entity.Services;

/// <inheritdoc cref="IBaseRepository<T>"/>
public abstract class BaseRepository<IModel, IDto>
  : IBaseRepository<IModel, IDto>
    where IModel : class, IBaseModel, new()
    where IDto : class, IBaseDto, new()
{
  protected readonly string c_getAllAsyncCacheKey = $"{typeof(IBaseModel)}/{nameof(GetAllAsync)}";
  protected readonly ApiTestContext m_context;
  protected readonly IMemoryCache m_cache;

  public BaseRepository(ApiTestContext context, IMemoryCache cache)
  {
    m_context = context;
    m_cache = cache;
  }

  /// <summary>Gets all entities asynchronously as data transfer objects.</summary>
  /// <returns>All entities as data transfer objects.</returns>
  public virtual async Task<IEnumerable<IDto>> GetAllAsync()
  {
    var dtos = await GetAllAsyncHelper();
    return dtos;
  }

  /// <summary>Gets an entity asynchronously as data transfer object.</summary>
  /// <param name="id">The id of a potentially stored entity.</param>
  /// <returns>
  /// An entity as data transfer object or null if entity with provided id is not found.
  /// </returns>
  public virtual async Task<IDto> GetByIdAsync(string id)
  {
    var dtos = await GetAllAsyncHelper();
    return dtos.FirstOrDefault(x => x.Id.Equals(id));
  }

  /// <summary>Adds an entity asynchronously to the database.</summary>
  /// <param name="dto">The data transfer object from which an entity can be created.</param>
  /// <returns>The id of the newly created entity.</returns>
  public virtual async Task<string> AddAsync(IDto dto)
  {
    var entity = MapToModel(dto);
    await m_context.Set<IModel>().AddAsync(entity);
    await m_context.SaveChangesAsync();
    ClearCache();
    return entity.Id;
  }

  /// <summary>
  /// Updates an entity with the provided data transfer object data.
  /// Note: Normally this method contains an id as the first argument, 
  ///   but i didn't want to mess with the predefined logic, so i left it like that.
  /// </summary>
  /// <param name="updatedEntityDto">The updated entity as data transfer object.</param>
  public virtual async Task UpdateAsync(IDto updatedEntityDto)
  {
    var dto = await GetByIdAsync(updatedEntityDto.Id);
    if (dto == null)
      return;

    var entityEntry = m_context.Set<IModel>().Entry(MapToModel(updatedEntityDto));
    entityEntry.State = EntityState.Modified;
    await m_context.SaveChangesAsync();
    ClearCache();
  }

  /// <summary>Deletes an entity with provided id.</summary>
  /// <param name="id">The id of the entity that is going to be deleted.</param>
  public virtual async Task DeleteAsync(string id)
  {
    var entity = await GetByIdAsync(id);
    if (entity == null)
      return;

    var entityEntry = m_context.Set<IModel>().Entry(new IModel { Id = id });
    entityEntry.State = EntityState.Deleted;
    await m_context.SaveChangesAsync();
    ClearCache();
  }

  /// <summary>Generates a data transfer object/contract from a database model.</summary>
  /// <param name="entity">The database entity object.</param>
  /// <returns>The data transfer object/contract object.</returns>
  public abstract IDto MapToDto(IModel model);

  // Manual mapping to EF model
  public abstract IModel MapToModel(IDto dto);

  private async Task<List<IDto>> GetAllAsyncHelper()
  {
    if (!m_cache.TryGetValue(c_getAllAsyncCacheKey, out List<IDto> dtos))
    {
      var entities = await m_context.Set<IModel>().ToListAsync();
      dtos = entities.Select(MapToDto).ToList();
      m_cache.Set(c_getAllAsyncCacheKey, dtos);
    }

    return dtos;
  }

  private void ClearCache() => m_cache.Remove(c_getAllAsyncCacheKey);
}
