using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SA51_CA_Project_Team10.Models
{
    public class GalleryViewModel
    {
        public int Columns { get; set; }
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int TotalProducts { get; set; }
        public List<Product> DisplayedProducts { get; set; }

        // Row and column here refer to the number of products that should be visible in a row and column respectively
        // Example: row = 3, column = 4 means that there should be 3 rows and 4 columns total in a page (12 products total)
        public GalleryViewModel(int row, int column, int page, List<Product> products)
        {
            Columns = column;
            Page = page;
            TotalProducts = products.Count;
            TotalPage = (int) Math.Ceiling((double)TotalProducts / (column * row));
            DisplayedProducts = new List<Product>();

            for (int i = (page - 1) * column * row; i < page * column * row && i < TotalProducts; i++)
            {
                DisplayedProducts.Add(products[i]);
            }
        }

        public GalleryViewModel(int page, List<Product> products) : this(4, 3, page, products) { }
    }
}
