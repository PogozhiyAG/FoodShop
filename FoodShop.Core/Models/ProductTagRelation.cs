using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class ProductTagRelation
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
