using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Database;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly Context _oDataDbContext;
        public OrdersController(Context oDataDbContext)
        {
            _oDataDbContext = oDataDbContext;
        }

        [EnableQuery()]
        public IEnumerable<Order> Get()
        {
            return _oDataDbContext.Orders;
        }

        [EnableQuery]
        public Order Get(long key, ODataQueryOptions<Order> queryOptions)
        {
            return queryOptions.ApplyTo(_oDataDbContext.Orders.Where(x => x.Orderkey == key)).FirstOrDefault<Order>();
        }
    }
}
