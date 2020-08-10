using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Presto.FunctionalTests
{
    public class PrestoTestStoreFactory : RelationalTestStoreFactory
    {
        public static PrestoTestStoreFactory Instance { get; } = new PrestoTestStoreFactory();

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkPresto();

        public override TestStore Create(string storeName)
            => PrestoTestStore.Create(storeName);

        public override TestStore GetOrCreate(string storeName)
            => PrestoTestStore.GetOrCreate(storeName);
    }
}
