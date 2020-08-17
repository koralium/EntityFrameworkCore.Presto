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
    public class CustomersController : ODataController
    {
        private readonly Context _oDataDbContext;
        public CustomersController(Context oDataDbContext)
        {
            _oDataDbContext = oDataDbContext;
        }

        [EnableQuery(PageSize = 2000)]
        public IEnumerable<Customer> Get(ODataQueryOptions<Customer> queryOptions)
        {
            return queryOptions.ApplyTo(_oDataDbContext.Customers).ToEnumerable<Customer>();
        }

        [EnableQuery]
        public Customer Get(long key, ODataQueryOptions<Customer> queryOptions)
        {
            return queryOptions.ApplyTo(_oDataDbContext.Customers.Where(x => x.Custkey == key)).FirstOrDefault<Customer>();
        }
    }
}
