using Honse.Engines.Common;
using Honse.Engines.Validation.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Honse.Engines.Validation
{
    public class OrderValidationEngine : IOrderValidationEngine
    {
        public void ValidatePlaceOrder(PlaceOrder order)
        {
            string errorMessage = "";

            if (order.RestaurantId == Guid.Empty)
                errorMessage += "RestaurantId is required!\n";

            if (string.IsNullOrEmpty(order.CustomerEmail))
                errorMessage += "Customer email is required!\n";
            else
            {
                ValidationContext validationContext = new ValidationContext(order) { MemberName = nameof(PlaceOrder.CustomerEmail) };
                if (!Validator.TryValidateProperty(order.CustomerEmail, validationContext, new List<ValidationResult>()))
                    errorMessage += "The email is not valid!\n";
            }

            if (string.IsNullOrEmpty(order.CustomerName))
                errorMessage += "Customer name is required!\n";

            if (string.IsNullOrEmpty(order.CustomerPhone))
                errorMessage += "Customer phone is required!\n";

            if (order.DeliveryAddress == null)
            {
                errorMessage += "Delivery address is required!\n";
            }
            else
            {
                if (string.IsNullOrEmpty(order.DeliveryAddress.Street))
                    errorMessage += "Street is required!\n";

                if (string.IsNullOrEmpty(order.DeliveryAddress.City))
                    errorMessage += "City is required!\n";
            }

            if (order.Products == null || order.Products.Count == 0)
                errorMessage += "At least one product is required!\n";
            else
            {
                foreach (var product in order.Products)
                {
                    if (product.ProductId == Guid.Empty)
                        errorMessage += "Product ID is required for all products!\n";

                    if (product.Quantity <= 0)
                        errorMessage += "Quantity must be greater than 0!\n";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);
        }
    }
}
