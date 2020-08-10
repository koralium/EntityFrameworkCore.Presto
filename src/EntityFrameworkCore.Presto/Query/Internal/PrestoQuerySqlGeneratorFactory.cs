using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Query.Internal
{
    public class PrestoQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {

        private readonly QuerySqlGeneratorDependencies _dependencies;

        public PrestoQuerySqlGeneratorFactory([NotNull] QuerySqlGeneratorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public QuerySqlGenerator Create()
            => new PrestoQuerySqlGenerator(_dependencies);
    }
}
