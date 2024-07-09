namespace Ir.ApiTest.Entity.Services.Interfaces;

/// <summary>
/// Stores generic data manipulation methods.
/// </summary>
public interface IBaseRepository<IModel, IDto>
{
  Task<IEnumerable<IDto>> GetAllAsync();

  Task<IDto> GetByIdAsync(string id);

  Task<string> AddAsync(IDto entity);

  Task UpdateAsync(IDto entity);

  Task DeleteAsync(string id);

  #region AutoMapper wannaby methods

  IDto MapToDto(IModel model);

  IModel MapToModel(IDto dto);

  #endregion AutoMapper wannaby methods
}