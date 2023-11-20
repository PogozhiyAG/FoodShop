using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class ProductTagRelation
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
