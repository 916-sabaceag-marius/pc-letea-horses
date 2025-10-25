
namespace Honse.Managers.Interface
{
    public interface IProductManager
    {
        Task<Product> AddProduct(CreateProductRequest request);

        Task<Product> GetProductById(Guid id, Guid userId);

        Task<Product> UpdateProduct(UpdateProductRequest request);

        Task<Product> DeleteProduct(Guid id, Guid userId);

        Task<List<Product>> GetAllProducts(Guid userId);

        //Task<List<Product>> SearchProducts();
    }

    public class Product
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
    }

    public class CreateProductRequest
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty; 

        public Guid CategoryId { get; set; }
    }

    public class UpdateProductRequest
    {
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public string Image { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
    }
}
