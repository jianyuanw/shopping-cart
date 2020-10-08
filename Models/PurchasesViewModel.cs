using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    // There is likely an easier way to implement this with proper usage of method syntax
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
        public DateTime DateTime { get; set; }
        public int Count { get; set; }
        public List<string> Ids;
    }
}
                                                         