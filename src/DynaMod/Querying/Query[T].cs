using DynaMod.Common;
using DynaMod.Mapping.Fluent.Fields;
using System;
using System.Linq.Expressions;
using static DynaMod.Common.Assertions;

namespace DynaMod.Querying
{
    public class Query<TEntity> : Query
    {
        internal Query() : base(typeof(TEntity))
        {
        }

        public Query<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            Assert(expression.Body is BinaryExpression, "Invalid expression");

            BinaryExpression binary_expression = (BinaryExpression)expression.Body;

            Assert(binary_expression.NodeType == ExpressionType.Equal, "Not implemented");

            Assert(binary_expression.Right.NodeType == ExpressionType.Constant, "Not implemented");

            FluentExpression left = ExpressionTree.GetFieldMapperFromExpression(binary_expression.Left);

            Assert(left != null, "Invalid expression");

            Conditions.Add(new Condition(left!, Operator.EQUALS, ((ConstantExpression)binary_expression.Right).Value));

            return this;
        }
    }
}
