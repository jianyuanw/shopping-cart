using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class Cart
    {
        public int Id { get; set; }
       
        public int Quantity { get; set; }
        
        public int SessionId { get; set; }

        public virtual Session Session { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }


    }
}
