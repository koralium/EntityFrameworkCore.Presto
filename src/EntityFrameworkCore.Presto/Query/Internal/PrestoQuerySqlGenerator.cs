using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.Presto.Query.Internal
{
    public class PrestoQuerySqlGenerator : QuerySqlGenerator
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        public PrestoQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) : base(dependencies)
        {
            _sqlGenerationHelper = dependencies.SqlGenerationHelper;
        }

        protected override Expression VisitTable(TableExpression tableExpression)
        {
            string catalog = null;
            string schema = null;
            int dotIndex = -1;
            if (tableExpression.Schema != null)
            {
                schema = tableExpression.Schema;
                dotIndex = tableExpression.Schema.IndexOf('.');
            }
            
            if(dotIndex >= 0)
            {
                catalog = tableExpression.Schema.Substring(0, dotIndex);
                schema = tableExpression.Schema.Substring(dotIndex + 1, tableExpression.Schema.Length - dotIndex - 1);
            }

            if(catalog != null)
            {
                Sql
                    .Append(_sqlGenerationHelper.DelimitIdentifier(catalog))
                    .Append('.')
                    .Append(_sqlGenerationHelper.DelimitIdentifier(tableExpression.Name, schema));
            }
            else
            {
                Sql
                    .Append(_sqlGenerationHelper.DelimitIdentifier(tableExpression.Name, tableExpression.Schema));
            }

            Sql
                .Append(AliasSeparator)
                .Append(_sqlGenerationHelper.DelimitIdentifier(tableExpression.Alias));
            
            return tableExpression;
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            if (selectExpression.Limit != null
                || selectExpression.Offset != null)
            {
                Sql.AppendLine()
                    .Append("LIMIT ");

                Visit(
                    selectExpression.Limit
                    ?? new SqlConstantExpression(Expression.Constant(-1), selectExpression.Offset.TypeMapping));

                if (selectExpression.Offset != null)
                {
                    Sql.Append(" OFFSET ");

                    Visit(selectExpression.Offset);
                }
            }
        }
    }
}
