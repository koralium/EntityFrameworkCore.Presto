using EntityFrameworkCore.Presto.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Infrastructure
{
    public class PrestoDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<PrestoDbContextOptionsBuilder, PrestoOptionsExtension>
    {
        public PrestoDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder) : base(optionsBuilder)
        {
        }
    }
}
