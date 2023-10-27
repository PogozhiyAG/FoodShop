using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class BasketItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Decimal Price { get; set; }
    }
}
