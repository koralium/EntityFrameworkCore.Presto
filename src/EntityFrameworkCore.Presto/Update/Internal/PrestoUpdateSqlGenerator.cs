using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Update.Internal
{
    public class PrestoUpdateSqlGenerator : UpdateSqlGenerator
    {
        public PrestoUpdateSqlGenerator(UpdateSqlGeneratorDependencies dependencies) : base(dependencies)
        {
        }

        protected override void AppendIdentityWhereCondition([NotNullAttribute] StringBuilder commandStringBuilder, [NotNullAttribute] ColumnModification columnModification)
        {
            throw new NotImplementedException();
        }

        protected override void AppendRowsAffectedWhereCondition([NotNullAttribute] StringBuilder commandStringBuilder, int expectedRowsAffected)
        {
            throw new NotImplementedException();
        }
    }
}
