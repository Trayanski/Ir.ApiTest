using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Models;

namespace Ir.ApiTest.Entity.Services.Interfaces;

/// <summary>
/// Stores data manipulation methods specific to the <see cref="Product"/> model.
/// </summary>
public interface IProductService : IBaseRepository<Product, ProductDto>
{
}