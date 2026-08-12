using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlantNode.Application.Interfaces;
using PlantNode.Domain.Entities.Products;

namespace PlantNode.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IPlantNodeDbContext _dbContext;

        public ProductService(IPlantNodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAllActiveProductsAsync()
        {
            return await _dbContext.Products
                .Include(p => p.MaterialType)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.MaterialType)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task SaveProductAsync(Product product)
        {
            if (product.Id == 0)
            {
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = DateTime.Now;
                _dbContext.Products.Add(product);
            }
            else
            {
                product.UpdatedDate = DateTime.Now;
                _dbContext.Products.Update(product);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedDate = DateTime.Now;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
