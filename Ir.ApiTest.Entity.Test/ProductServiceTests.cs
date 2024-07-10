using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Models;
using Ir.ApiTest.Entity.Services;
using Ir.ApiTest.Entity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Ir.ApiTest.Entity.Test;

[TestClass]
public class ProductServiceTests
{
  [TestMethod, TestCategory("UnitTest")]
  public async Task AddAsync_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    var productDto = new ProductDto() { Id = c_productId };
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    repositoryMock
      .Setup(x => x.AddAsync(It.IsAny<ProductDto>()))
      .ReturnsAsync(c_productId);
    var productService = new ProductService(repositoryMock.Object);

    // Act
    var result = await productService.AddAsync(productDto);

    // Assert
    Assert.AreEqual(c_productId, result);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task DeleteAsync_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    var productDto = new ProductDto() { Id = c_productId };
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    repositoryMock
      .Setup(x => x.DeleteAsync(c_productId));
    var productService = new ProductService(repositoryMock.Object);

    // Act
    // Assert
    await productService.DeleteAsync(c_productId);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task GetAllAsync_HappyDay_Success()
  {
    // Arange
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    var productDtos = new List<ProductDto>() { new() { Id = "ProductId" } };
    repositoryMock
      .Setup(x => x.GetAllAsync())
      .ReturnsAsync(productDtos);
    var productService = new ProductService(repositoryMock.Object);

    // Act
    var result = await productService.GetAllAsync();

    // Assert
    Assert.AreEqual(productDtos, result);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task GetByIdAsync_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    var productDto = new ProductDto { Id = c_productId };
    repositoryMock
      .Setup(x => x.GetByIdAsync(c_productId))
      .ReturnsAsync(productDto);
    var productService = new ProductService(repositoryMock.Object);

    // Act
    var result = await productService.GetByIdAsync(c_productId);

    // Assert
    Assert.AreEqual(productDto, result);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task UpdateAsync_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    DateTime expectedLastUpdated = DateTime.Now;
    DateTime expectedCreated = DateTime.Now.AddDays(-1);
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    var productDto = new ProductDto
    {
      Id = c_productId,
      Created = expectedCreated,
      LastUpdated = expectedLastUpdated.AddDays(-1)
    };
    repositoryMock
      .Setup(x => x.GetByIdAsync(c_productId))
      .ReturnsAsync(productDto);
    repositoryMock
      .Setup(x => x.UpdateAsync(productDto));
    var productService = new ProductService(repositoryMock.Object);

    // Act
    await productService.UpdateAsync(productDto);

    // Assert
    Assert.AreEqual(expectedCreated, productDto.Created);
    Assert.AreEqual(expectedLastUpdated.Year, productDto.LastUpdated.Year);
    Assert.AreEqual(expectedLastUpdated.Month, productDto.LastUpdated.Month);
    Assert.AreEqual(expectedLastUpdated.Day, productDto.LastUpdated.Day);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task UpdateAsync_EntryDoesNotExist_Throws()
  {
    // Arange
    const string c_productId = "ProductId";
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    repositoryMock
      .Setup(x => x.GetByIdAsync(c_productId))
      .ReturnsAsync((ProductDto)null);
    var productDto = new ProductDto { Id = c_productId };
    repositoryMock
      .Setup(x => x.UpdateAsync(productDto));
    var productService = new ProductService(repositoryMock.Object);

    // Act
    var action = async () => await productService.UpdateAsync(productDto);

    // Assert (verify exception is thrown)
    await Assert.ThrowsExceptionAsync<TaskCanceledException>(action, $"Product with ID '{productDto.Id}' not found.");
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task UpdateAsync_AlteredCreated_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    DateTime expectedCreated = DateTime.Now.AddDays(-1);
    DateTime alteredCreated = DateTime.Now;
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    repositoryMock
      .Setup(x => x.GetByIdAsync(c_productId))
      .ReturnsAsync(new ProductDto
      {
        Id = c_productId,
        Created = expectedCreated,
      });
    var productDto = new ProductDto
    {
      Id = c_productId,
      Created = alteredCreated,
    };
    repositoryMock
      .Setup(x => x.UpdateAsync(productDto));
    var productService = new ProductService(repositoryMock.Object);

    // Act
    await productService.UpdateAsync(productDto);

    // Assert
    Assert.AreEqual(expectedCreated, productDto.Created);
  }

  [TestMethod, TestCategory("UnitTest")]
  public async Task UpdateAsync_AlteredLastUpdated_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    DateTime previouslyStoredLastUpdated = DateTime.Now.AddDays(-1);
    DateTime expectedLastUpdated = DateTime.Now;
    DateTime alteredLastUpdated = DateTime.Now.AddDays(1);
    var repositoryMock = new Mock<IBaseRepository<Product, ProductDto>>();
    repositoryMock
      .Setup(x => x.GetByIdAsync(c_productId))
      .ReturnsAsync(new ProductDto
      {
        Id = c_productId,
        LastUpdated = previouslyStoredLastUpdated,
      });
    var productDto = new ProductDto
    {
      Id = c_productId,
      LastUpdated = alteredLastUpdated,
    };
    repositoryMock
      .Setup(x => x.UpdateAsync(productDto));
    var productService = new ProductService(repositoryMock.Object);

    // Act
    await productService.UpdateAsync(productDto);

    // Assert
    Assert.AreEqual(expectedLastUpdated.Year, productDto.LastUpdated.Year);
    Assert.AreEqual(expectedLastUpdated.Month, productDto.LastUpdated.Month);
    Assert.AreEqual(expectedLastUpdated.Day, productDto.LastUpdated.Day);
  }

  [TestMethod, TestCategory("UnitTest")]
  public void MapToDto_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    var productModel = new Product
    {
      Id = c_productId,
      Size = It.IsAny<string>(),
      Colour = It.IsAny<string>(),
      Name = It.IsAny<string>(),
      Price = It.IsAny<double>(),
      LastUpdated = It.IsAny<DateTimeOffset>(),
      Created = It.IsAny<DateTimeOffset>(),
      Hash = It.IsAny<string>()
    };
    var expectedDto = new ProductDto
    {
      Id = productModel.Id,
      Size = productModel.Size,
      Colour = productModel.Colour,
      Name = productModel.Name,
      Price = productModel.Price,
      LastUpdated = productModel.LastUpdated,
      Created = productModel.Created,
      Hash = productModel.Hash
    };
    var apiTestContextMock = new Mock<ApiTestContext>(new DbContextOptions<ApiTestContext>());
    var memoryCacheMock = new Mock<IMemoryCache>();
    var productService = new ProductService(
      repository: new ProductRepository(apiTestContextMock.Object, memoryCacheMock.Object));

    // Act
    var actualDto = productService.MapToDto(productModel);

    // Assert
    Assert.AreEqual(expectedDto.Id, actualDto.Id);
    Assert.AreEqual(expectedDto.Size, actualDto.Size);
    Assert.AreEqual(expectedDto.Colour, actualDto.Colour);
    Assert.AreEqual(expectedDto.Name, actualDto.Name);
    Assert.AreEqual(expectedDto.Price, actualDto.Price);
    Assert.AreEqual(expectedDto.LastUpdated, actualDto.LastUpdated);
    Assert.AreEqual(expectedDto.Created, actualDto.Created);
    Assert.AreEqual(expectedDto.Hash, actualDto.Hash);
  }

  [TestMethod, TestCategory("UnitTest")]
  public void MapToModel_HappyDay_Success()
  {
    // Arange
    const string c_productId = "ProductId";
    var productDto = new ProductDto
    {
      Id = c_productId,
      Size = It.IsAny<string>(),
      Colour = It.IsAny<string>(),
      Name = It.IsAny<string>(),
      Price = It.IsAny<double>(),
      LastUpdated = It.IsAny<DateTimeOffset>(),
      Created = It.IsAny<DateTimeOffset>(),
      Hash = It.IsAny<string>()
    };
    var expectedModel = new Product
    {
      Id = productDto.Id,
      Size = productDto.Size,
      Colour = productDto.Colour,
      Name = productDto.Name,
      Price = productDto.Price,
      LastUpdated = productDto.LastUpdated,
      Created = productDto.Created,
      Hash = productDto.Hash
    };
    var apiTestContextMock = new Mock<ApiTestContext>(new DbContextOptions<ApiTestContext>());
    var memoryCacheMock = new Mock<IMemoryCache>();
    var productService = new ProductService(
      repository: new ProductRepository(apiTestContextMock.Object, memoryCacheMock.Object));

    // Act
    var actualModel = productService.MapToModel(productDto);

    // Assert
    Assert.AreEqual(expectedModel.Id, actualModel.Id);
    Assert.AreEqual(expectedModel.Size, actualModel.Size);
    Assert.AreEqual(expectedModel.Colour, actualModel.Colour);
    Assert.AreEqual(expectedModel.Name, actualModel.Name);
    Assert.AreEqual(expectedModel.Price, actualModel.Price);
    Assert.AreEqual(expectedModel.LastUpdated, actualModel.LastUpdated);
    Assert.AreEqual(expectedModel.Created, actualModel.Created);
    Assert.AreEqual(expectedModel.Hash, actualModel.Hash);
  }
}