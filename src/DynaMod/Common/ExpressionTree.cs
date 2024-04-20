using DynaMod.Mapping.Fluent.Fields;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using static DynaMod.Common.Assertions;

namespace DynaMod.Common
{
    internal class ExpressionTree
    {
        internal static FluentExpression GetFieldMapperFromExpression(Expression expressionBody)
        {
            if (expressionBody is UnaryExpression u)
            {
                expressionBody = u.Operand;
            }

            if (expressionBody is MemberExpression memberExpression)
            {
                var stack = new Stack<MemberInfo>();

                Expression expr = memberExpression;

                while (expr != null)
                {
                    if (expr is MemberExpression mexpr)
                    {
                        stack.Push(mexpr.Member);
                        expr = mexpr.Expression;
                        continue;
                    }
                    else if (expr is ParameterExpression)
                    {
                        break;
                    }
                    else
                    {
                        throw new InvalidOperationException("The provided expression is not supported");
                    }
                }

                FluentMemberAccessExpression node = null;

                while (stack.Count > 0)
                {
                    var member = stack.Pop();
                    var getter = Reflection.GetGetter(member.DeclaringType, member.Name);

                    Assert(getter != null, $"Getter for property {member.Name} could not be not found");


                    var n = new FluentMemberAccessExpression(member.Name, getter);

                    if (node is null)
                    {
                        node = n;
                    }
                    else
                    {
                        node.AddChild(n);
                    }
                }

                return node!;
            }
            else if (expressionBody is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.DeclaringType == typeof(string) &&
                    methodCallExpression.Method.Name == nameof(string.Format))
                {
                    var stringFormat = (ConstantExpression)methodCallExpression.Arguments[0];

                    var mapper = new FluentStringFormatExpression((string)stringFormat.Value!);

                    for (int i = 1; i < methodCallExpression.Arguments.Count; i++)
                    {
                        var argMapper = GetFieldMapperFromExpression(methodCallExpression.Arguments[i]);
                        mapper.Children.Add(argMapper);
                    }
                    return mapper;

                }
                else
                {
                    throw new InvalidOperationException("The provided expression is not supported");
                }
            } else if (expressionBody is ConstantExpression constant_expression)
            {
                return new FluentConstantExpression(constant_expression.Value);
            }

            throw new InvalidOperationException("The provided expression is not supported");
        }
    }
}
