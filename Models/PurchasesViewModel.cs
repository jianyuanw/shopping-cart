using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class PurchasesViewModel
    {
        public List<ConciseOrderDetail> _products;

        public PurchasesViewModel()
        {
            _products = new List<ConciseOrderDetail>();
        }

    }

    public class ConciseOrderDetail
    {
        public string ImageLink { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public List<string> Ids;
    }
}
