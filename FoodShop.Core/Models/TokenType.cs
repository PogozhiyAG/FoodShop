using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class TokenType : EntityBase
    {
        public string Code { get; set; }
        public string? Decription { get; set; }
        public decimal OfferPriority { get; set; }
    }
}
