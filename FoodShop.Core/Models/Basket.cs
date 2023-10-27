using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class Basket
    {
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new ();

    }
}
