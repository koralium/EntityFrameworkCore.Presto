using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Customer
    {
        public long Custkey { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public long Nationkey { get; set; }

        public string Phone { get; set; }

        public double Acctbal { get; set; }

        public string Mktsegment { get; set; }

        public string Comment { get; set; }
    }
}
