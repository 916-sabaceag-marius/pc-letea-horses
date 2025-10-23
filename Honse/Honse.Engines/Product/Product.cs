using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honse.Engines.Product
{
    public class Product
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal VAT { get; set; }

        //[ForeignKey("Category")]
        [NotMapped]
        public Guid CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
