using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Update.Internal
{
    public class PrestoModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        private readonly ModificationCommandBatchFactoryDependencies _dependencies;

        public PrestoModificationCommandBatchFactory(
              ModificationCommandBatchFactoryDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public ModificationCommandBatch Create()
        {
            return new SingularModificationCommandBatch(_dependencies);
        }
    }
}
