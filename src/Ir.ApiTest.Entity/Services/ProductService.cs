using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Models;
using Ir.ApiTest.Entity.Services.Interfaces;

namespace Ir.ApiTest.Entity.Services;

/// <inheritdoc cref="IProductService"/>
public class ProductService : IProductService
{
  private readonly IBaseRepository<Product, ProductDto> m_repository;

  public ProductService(IBaseRepository<Product, ProductDto> repository)
  {
    m_repository = repository;
  }

  public async Task<string> AddAsync(ProductDto dto) => await m_repository.AddAsync(dto);
  public Task DeleteAsync(string id) => m_repository.DeleteAsync(id);
  public Task<IEnumerable<ProductDto>> GetAllAsync() => m_repository.GetAllAsync();
  public Task<ProductDto> GetByIdAsync(string id) => m_repository.GetByIdAsync(id);
  public Task UpdateAsync(ProductDto dto) => m_repository.UpdateAsync(dto);
  public ProductDto MapToDto(Product model) => m_repository.MapToDto(model);
  public Product MapToModel(ProductDto dto) => m_repository.MapToModel(dto);
}