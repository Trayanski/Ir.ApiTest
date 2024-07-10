using Ir.ApiTest.Contracts;
using Ir.ApiTest.Controllers;
using Ir.ApiTest.Entity.Models;
using Ir.ApiTest.Entity.Services;
using Ir.ApiTest.Entity.Services.Interfaces;
using Moq;

namespace Ir.ApiTest.Test;

[TestClass]
public class ProductsControllerTests
{
  [TestMethod, TestCategory("UnitTest")]
  public async Task GetProducts_HappyDay_Successful()
  {
    // Arragne
    var productDtos = new List<ProductDto>
    {
      new() { Id = "FirstProduct"},
      new() { Id = "SecondProduct" }
    };
    var baseRepositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    baseRepositoryMock
      .Setup(x => x.GetAllAsync())
      .ReturnsAsync(productDtos);
    var productServiceMock = new Mock<ProductService>(baseRepositoryMock.Object);
    var productsController = new ProductsController(productServiceMock.Object);

    // Act
    var actualProductDtos = await productsController.GetProducts();

    // Assert
    Assert.IsNotNull(actualProductDtos?.Result);
    // TODO: complete test and add tests for the other public controller methods
  }
}