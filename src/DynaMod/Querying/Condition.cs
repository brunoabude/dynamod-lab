using DynaMod.Mapping.Fluent.Fields;
using System;

namespace DynaMod.Querying
{
    internal enum Operator
    {
        EQUALS
    }
    internal class Condition
    {
        internal readonly FluentExpression fieldMapper;
        internal readonly Operator op;
        internal readonly object value;

        public Condition(FluentExpression fieldMapper, Operator op, object value)
        {
            this.fieldMapper = fieldMapper ?? throw new ArgumentNullException(nameof(fieldMapper));
            this.op = op;
            this.value = value;
        }
    }
}
