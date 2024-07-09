using Ir.ApiTest.Entity.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Ir.ApiTest.Entity.Models;

public class Product : IBaseModel
{
  [Key]
  public string Id { get; set; }

  [Required]
  public string Size { get; set; }

  [Required]
  public string Colour { get; set; }

  [Required]
  public string Name { get; set; }

  [Required]
  public double Price { get; set; }

  [Required]
  public DateTimeOffset LastUpdated { get; set; }

  [Required]
  public DateTimeOffset Created { get; set; }

  [Required]
  public string Hash { get; set; }
};