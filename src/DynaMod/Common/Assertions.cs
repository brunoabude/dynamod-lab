using DynaMod.Serialization.Document;
using System;

namespace DynaMod.Common
{
    internal static class Assertions
    {
        internal static void Assert(bool expression, string message)
        {
            if (!expression)
            {
                throw new InvalidOperationException(message);
            }
        }
        internal static void Assert(bool expression)
        {
            if (!expression)
            {
                throw new InvalidOperationException();
            }
        }

        internal static void AssertSyntax(bool expression, string message)
        {
            if (!expression)
            {
                throw new InvalidSyntaxException(message);
            }
        }

        internal static void AssertArgument(bool expression, string message, string paramName = null)
        {
            if (!expression)
            {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}
