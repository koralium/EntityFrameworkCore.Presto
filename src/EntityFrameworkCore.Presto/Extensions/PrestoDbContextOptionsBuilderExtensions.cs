using EntityFrameworkCore.Presto.Infrastructure;
using EntityFrameworkCore.Presto.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public static class PrestoDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UsePresto(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            Action<PrestoDbContextOptionsBuilder> prestoOptionsAction = null)
        {

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(GetOrCreateExtension(optionsBuilder));

            prestoOptionsAction?.Invoke(new PrestoDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UsePresto(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            Action<PrestoDbContextOptionsBuilder> sqliteOptionsAction = null)
        {
            var extension = (PrestoOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            sqliteOptionsAction?.Invoke(new PrestoDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        private static PrestoOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder options)
            => options.Options.FindExtension<PrestoOptionsExtension>()
                ?? new PrestoOptionsExtension();
    }
}
