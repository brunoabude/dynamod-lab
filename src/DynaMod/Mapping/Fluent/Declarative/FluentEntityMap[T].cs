using DynaMod.Common;
using DynaMod.Mapping.Fluent.Fields;
using System;
using System.Linq.Expressions;

namespace DynaMod.Mapping.Fluent.Declarative
{
    public class FluentEntityMap<TEntity> : EntityMap
    {
        protected internal override void EnsureInitialized()
        {
            base.EnsureInitialized();
        }

        public void Table(string tableName)
        {
            SetTableName(tableName);
        }

        public void PartitionKey(Expression<Func<TEntity, object>> partionKeyExpression)
        {
            partition_key_expression = ExpressionTree.GetFieldMapperFromExpression(partionKeyExpression.Body);
        }

        public void SortKey(Expression<Func<TEntity, object>> sortKeyExpression)
        {
            sort_key_expression = ExpressionTree.GetFieldMapperFromExpression(sortKeyExpression.Body);
        }

        public void Unique(Expression<Func<TEntity, object>> field_expression)
        {
            var member_expression = ExpressionTree.GetFieldMapperFromExpression(field_expression.Body);

            if (member_expression.NodeType != Fields.FluentExpressionType.MemberAccess)
                throw new ArgumentException($"Only member expressions are accepted");

            AddConstraint(((FluentMemberAccessExpression)member_expression).MemberName, Constraints.Constraint.Unique);
        }
    }
}
