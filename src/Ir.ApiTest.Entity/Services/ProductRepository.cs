using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Ir.ApiTest.Entity.Services;

public class ProductRepository : BaseRepository<Product, ProductDto>
{
  public ProductRepository(ApiTestContext context, IMemoryCache cache) : base(context, cache)
  {
  }

  #region AutoMapper wannaby methods

  public override ProductDto MapToDto(Product entity)
  {
    return new ProductDto
    {
      Id = entity.Id,
      Size = entity.Size,
      Colour = entity.Colour,
      Name = entity.Name,
      Price = entity.Price,
      LastUpdated = entity.LastUpdated,
      Created = entity.Created,
      Hash = entity.Hash
    };
  }

  public override Product MapToModel(ProductDto dto)
  {
    return new Product
    {
      Id = dto.Id,
      Size = dto.Size,
      Colour = dto.Colour,
      Name = dto.Name,
      Price = dto.Price,
      LastUpdated = dto.LastUpdated,
      Created = dto.Created,
      Hash = dto.Hash
    };
  }

  #endregion AutoMapper wannaby methods
}
