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

  public override ProductDto MapToDto(Product model)
  {
    return new ProductDto
    {
      Id = model.Id,
      Size = model.Size,
      Colour = model.Colour,
      Name = model.Name,
      Price = model.Price,
      LastUpdated = model.LastUpdated,
      Created = model.Created,
      Hash = model.Hash
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
