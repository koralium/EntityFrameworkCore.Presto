using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.Presto.Query.Internal
{
    public class PrestoSqlTranslatingExpressionVisitor : RelationalSqlTranslatingExpressionVisitor
    {
        public PrestoSqlTranslatingExpressionVisitor(RelationalSqlTranslatingExpressionVisitorDependencies dependencies, IModel model, QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor) : base(dependencies, model, queryableMethodTranslatingExpressionVisitor)
        {
        }

        public override SqlExpression TranslateCount(Expression expression = null)
        {
            return base.TranslateCount(expression);
        }
    }
}
