using Honse.Engines.Common;
using Honse.Engines.Validation.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Validation
{
    public class ProductValidationEngine : IProductValidationEngine
    {
        public void ValidateCreateProduct(CreateProduct product)
        {
            string errorMessage = "";

            if (product.UserId == Guid.Empty)
                errorMessage += "UserId is required!\n";

            if (product.CategoryId == Guid.Empty)
                errorMessage += "CategoryId is required!\n";

            if (string.IsNullOrEmpty(product.Name))
                errorMessage += "Product name is required!\n";

            if (!product.Name.All(char.IsLetterOrDigit))
                errorMessage += "Product name is not valid!\n";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);
        }
    }
}
