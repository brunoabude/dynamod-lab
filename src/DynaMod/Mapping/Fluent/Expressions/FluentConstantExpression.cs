namespace DynaMod.Mapping.Fluent.Fields
{
    internal class FluentConstantExpression : FluentExpression
    {
        internal readonly object Constant;

        internal FluentConstantExpression(object constant) : base(FluentExpressionType.ConstantExpression)
        {
            Constant = constant;
        }

        internal override object Apply(object instance)
        {
            return Constant;
        }

        internal override bool IsEqual(FluentExpression other)
        {
            if (other.NodeType != FluentExpressionType.ConstantExpression)
                return false;

            var otherConstant = (FluentConstantExpression)other;

            return Constant == otherConstant.Constant;
        }
    }
}
