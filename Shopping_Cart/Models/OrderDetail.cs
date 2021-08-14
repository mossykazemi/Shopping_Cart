using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_Cart.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailtId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Price { get; set; }
        
        [Required] 
        public int Count { get; set; }
        
        public Order order { get; set; }
        public Product Product { get; set; }
        
    }
}
