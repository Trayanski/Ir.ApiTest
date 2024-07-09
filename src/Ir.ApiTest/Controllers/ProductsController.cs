using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ir.ApiTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductService m_productService;

  public ProductsController(IProductService productService)
  {
    m_productService = productService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
  {
    var productDto = await m_productService.GetAllAsync();
    if (productDto == null)
      return NotFound();

    return Ok(productDto);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetProduct([FromRoute] string id)
  {
    var productDto = await m_productService.GetByIdAsync(id);
    if (productDto == null)
      return NotFound();

    return Ok(productDto);
  }

  [HttpPost()]
  public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    // TODO: I have validation later on but I think I need some kind of validation here as well.

    try
    {
      var createdProductId = await m_productService.AddAsync(productDto);
      return Ok(createdProductId);
    }
    catch (ValidationException ex)
    {
      return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }

  [HttpPatch("{id}")]
  public async Task<IActionResult> UpdateProduct([FromBody] JsonPatchDocument<ProductDto> productPatchDocument)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var productDto = new ProductDto();
    productPatchDocument.ApplyTo(productDto);

    // TODO: I have validation later on but I think I need some kind of validation here as well.

    var foundProductDto = await m_productService.GetByIdAsync(productDto.Id);
    if (foundProductDto == null)
      return NotFound();

    try
    {
      await m_productService.UpdateAsync(productDto);
      return NoContent();
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex.Message);
    }
  }
}