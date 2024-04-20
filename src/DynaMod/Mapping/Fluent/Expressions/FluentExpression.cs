using System;
using System.Collections.Generic;

namespace DynaMod.Mapping.Fluent.Fields
{
    internal abstract class FluentExpression
    {
        internal FluentExpressionType NodeType;
        internal IList<FluentExpression> Children;

        protected FluentExpression(FluentExpressionType nodeType)
        {
            NodeType = nodeType;
            Children = new List<FluentExpression>();
        }

        internal abstract bool IsEqual(FluentExpression other);

        internal void AddChild(FluentExpression node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            Children.Add(node);
        }

        internal abstract object Apply(object instance);
    }
}
