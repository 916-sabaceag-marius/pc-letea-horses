
using Honse.Engines.Filtering.Product;
using Honse.Managers.Interface;
using Honse.Resources.Interface;

namespace Honse.Managers
{
    public class ProductManager : Interface.IProductManager
    {
        private readonly IProductResource productResource;
        private readonly ProductFilteringEngine productFilteringEngine;

        public ProductManager(
            Resources.Interface.IProductResource productResource,
            Engines.Filtering.Product.ProductFilteringEngine productFilteringEngine)
        {
            this.productResource = productResource;
            this.productFilteringEngine = productFilteringEngine;
        }

        public Task<Interface.Product> AddProduct(CreateProductRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Interface.Product> DeleteProduct(Guid id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Interface.Product>> GetAllProducts(Guid userId)
        {
            var specification = productFilteringEngine.ToSpecification(new Engines.Filtering.Interfaces.ProductFilterRequest
            {
                UserId = userId
            });

            return await productResource.Filter(specification, 100, 1);
        }

        public Task<Interface.Product> GetProductById(Guid id, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Interface.Product> UpdateProduct(UpdateProductRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
