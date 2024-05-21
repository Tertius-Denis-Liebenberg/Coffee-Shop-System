using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeShopSystem
{
    // Order class to get and set values
    public class Orders
    {
        public string CustomerID { get; set; }
        public string CoffeeType { get; set; }
        public string Size { get; set; }
        public string Sugar { get; set; }
        public string Cream { get; set; }
        public string Quantity {get; set; }
        public string Dates { get; set; }
        public string Price { get; set; }

    }
}
