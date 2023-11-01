using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Core.Models
{
    public class UserToken : EntityBase
    {
        public string UserId { get; set; }
        public Guid TokenTypeId { get; set; }
        public TokenType TokenType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
