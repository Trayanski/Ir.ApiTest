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

  public async Task<string> AddAsync(ProductDto entity) => await m_repository.AddAsync(entity);
  public Task DeleteAsync(string id) => m_repository.DeleteAsync(id);
  public Task<IEnumerable<ProductDto>> GetAllAsync() => m_repository.GetAllAsync();
  public Task<ProductDto> GetByIdAsync(string id) => m_repository.GetByIdAsync(id);
  public Task UpdateAsync(ProductDto entity) => m_repository.UpdateAsync(entity);
  public ProductDto MapToDto(Product entity) => m_repository.MapToDto(entity);
  public Product MapToModel(ProductDto entity) => m_repository.MapToModel(entity);
}