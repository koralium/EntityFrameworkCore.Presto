using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Order
    {
        public long Orderkey { get; set; }

        public long Custkey { get; set; }

        public string Orderstatus { get; set; }

        public double Totalprice { get; set; }

        public DateTime Orderdate { get; set; }

        public string Orderpriority { get; set; }

        public string Clerk { get; set; }

        public int Shippriority { get; set; }

        public string Comment { get; set; }
    }
}
