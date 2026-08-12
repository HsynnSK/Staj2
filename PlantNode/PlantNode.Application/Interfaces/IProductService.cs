using System.Collections.Generic;
using System.Threading.Tasks;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.Application.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllActiveProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task SaveProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
