using System;
using System.Collections.Generic;

namespace Honse.Engines.Common
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
