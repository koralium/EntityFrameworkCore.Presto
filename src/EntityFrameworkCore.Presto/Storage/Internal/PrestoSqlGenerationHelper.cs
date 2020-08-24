using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        public PrestoSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies) : base(dependencies)
        {
        }
    }
}
