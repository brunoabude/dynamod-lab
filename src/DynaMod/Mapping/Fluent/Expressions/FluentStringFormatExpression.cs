using System;

namespace DynaMod.Mapping.Fluent.Fields
{
    internal class FluentStringFormatExpression : FluentExpression
    {
        internal readonly string FormatString;

        internal FluentStringFormatExpression(string formatString) : base(FluentExpressionType.StringFormatMethod)
        {
            if (string.IsNullOrWhiteSpace(formatString))
            {
                throw new ArgumentException($"'{nameof(formatString)}' cannot be null or whitespace.", nameof(formatString));
            }

            FormatString = formatString;
        }

        internal override object Apply(object instance)
        {
            if (instance is null)
                return null;

            var args = new object[Children.Count];

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = Children[i].Apply(instance);
            }

            return string.Format(FormatString, args);
        }

        internal string MapFromArgs(params object[] args)
        {
            return string.Format(FormatString, args);
        }

        internal override bool IsEqual(FluentExpression other)
        {
            if (other.NodeType != FluentExpressionType.StringFormatMethod)
                return false;

            var otherFormatString = (FluentStringFormatExpression)other;

            if (FormatString != otherFormatString.FormatString)
                return false;

            if (Children.Count != otherFormatString.Children.Count)
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
