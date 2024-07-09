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

  public virtual async Task<IEnumerable<IDto>> GetAllAsync()
  {
    var entities = await GetAllAsyncHelper();
    return entities;
  }

  public virtual async Task<IDto> GetByIdAsync(string id)
  {
    var entities = await GetAllAsyncHelper();
    return entities.FirstOrDefault(x => x.Id.Equals(id));
  }

  public virtual async Task<string> AddAsync(IDto dto)
  {
    var entity = MapToModel(dto);
    await m_context.Set<IModel>().AddAsync(entity);
    await m_context.SaveChangesAsync();
    ClearCache();
    return entity.Id;
  }

  public virtual async Task UpdateAsync(IDto updatedEntity)
  {
    var entity = await GetByIdAsync(updatedEntity.Id);
    if (entity == null)
      return;

    var entityEntry = m_context.Set<IModel>().Entry(MapToModel(updatedEntity));
    entityEntry.State = EntityState.Modified;
    await m_context.SaveChangesAsync();
    ClearCache();
  }

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
  public abstract IDto MapToDto(IModel entity);

  // Manual mapping to EF model
  public abstract IModel MapToModel(IDto entity);

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
