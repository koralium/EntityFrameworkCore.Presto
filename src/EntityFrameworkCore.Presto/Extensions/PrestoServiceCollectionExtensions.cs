using EntityFrameworkCore.Presto.Diagnostics.Internal;
using EntityFrameworkCore.Presto.Infrastructure.Internal;
using EntityFrameworkCore.Presto.Query.Internal;
using EntityFrameworkCore.Presto.Storage.Internal;
using EntityFrameworkCore.Presto.Update.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PrestoServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkPresto([NotNull] this IServiceCollection serviceCollection)
        {
            var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
                .TryAdd<LoggingDefinitions, PrestoLoggingDefinitions>()
                .TryAdd<IDatabaseProvider, DatabaseProvider<PrestoOptionsExtension>>()
                .TryAdd<IRelationalTypeMappingSource, PrestoTypeMappingSource>()
                .TryAdd<ISqlGenerationHelper, PrestoSqlGenerationHelper>()
                .TryAdd<IModificationCommandBatchFactory, PrestoModificationCommandBatchFactory>()
                .TryAdd<IUpdateSqlGenerator, PrestoUpdateSqlGenerator>()
                .TryAdd<IQuerySqlGeneratorFactory, PrestoQuerySqlGeneratorFactory>()
                .TryAdd<IQueryableMethodTranslatingExpressionVisitorFactory, PrestoQueryableMethodTranslatingExpressionVisitorFactory>()
                .TryAdd<IRelationalConnection, PrestoRelationalConnection>()

                //New query pipeline
                .TryAdd<IRelationalSqlTranslatingExpressionVisitorFactory, PrestoSqlTranslatingExpressionVisitorFactory>();

            builder.TryAddCoreServices();

            return serviceCollection;
        }
    }
}
