using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Presto.FunctionalTests
{
    public class PrestoTestStore : RelationalTestStore
    {

        public static PrestoTestStore Create(string name)
            => new PrestoTestStore(name, shared: false);

        public static PrestoTestStore GetOrCreate(string name)
            => new PrestoTestStore(name, false);

        public PrestoTestStore(string name, bool shared) : base(name, shared)
        {
        }

        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
            => builder.UsePresto("localhost:8080; Catalog=tpch; Schema=tiny;");

        public override void Clean(DbContext context)
        {
            throw new NotImplementedException();
        }
    }
}
