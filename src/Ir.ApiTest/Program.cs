using Ir.ApiTest.Contracts;
using Ir.ApiTest.Entity;
using Ir.ApiTest.Entity.Models;
using Ir.ApiTest.Entity.Services;
using Ir.ApiTest.Entity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
  options.SerializerSettings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
  options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiTestContext>(options => options.UseInMemoryDatabase(databaseName: "Database"));
builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IBaseRepository<Product, ProductDto>, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();