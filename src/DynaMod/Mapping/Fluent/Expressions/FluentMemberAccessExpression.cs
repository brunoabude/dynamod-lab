using System;
using System.Reflection;

namespace DynaMod.Mapping.Fluent.Fields
{
    internal class FluentMemberAccessExpression : FluentExpression
    {
        internal readonly string MemberName;
        internal readonly MethodInfo MemberGetter;

        internal FluentMemberAccessExpression(string memberName, MethodInfo memberGetter) : base(FluentExpressionType.MemberAccess)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException($"'{nameof(memberName)}' cannot be null or whitespace.", nameof(memberName));
            }

            MemberName = memberName;
            MemberGetter = memberGetter ?? throw new ArgumentNullException(nameof(memberGetter));
        }

        internal override object Apply(object instance)
        {
            if (instance is null)
                return null;

            var value = MemberGetter.Invoke(instance, null);

            foreach (var c in Children)
            {
                value = c.Apply(value);
            }

            return value;
        }

        internal override bool IsEqual(FluentExpression other)
        {
            if (other.NodeType != FluentExpressionType.MemberAccess)
                return false;

            var otherMember = (FluentMemberAccessExpression)other;

            if (MemberName != otherMember.MemberName)
                return false;

            if (Children.Count != otherMember.Children.Count)
                return false;

            if (Children.Count > 0)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (!Children[i].IsEqual(other.Children[i]))
                        return false;
                }
            }

            return true;
        }
    }
}
