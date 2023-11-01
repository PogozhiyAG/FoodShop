using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class ProductPriceStrategy : EntityBase
    {
        public static ProductPriceStrategy Default { get; } = new() { Name = "Default", Quantity = 1, Rate = 1.0M };
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }

        public decimal GetAmount(decimal price, int quantity)
        {
            var Q = Quantity != 0 ? Quantity : 1;
            return Math.Round((quantity / Q) * Q * Rate * price + (quantity % Q) * price, 2);
        }
    }
}
