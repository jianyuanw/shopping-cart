using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SA51_CA_Project_Team10.Models
{
    public class ActivationCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int OrderDetailsId { get; set; }
        public virtual OrderDetail OrderDetails { get; set; }
    }
}
