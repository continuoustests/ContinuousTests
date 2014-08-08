using System.Linq.Expressions;
using System.Collections.Generic;

namespace PowerAssert.Infrastructure
{
    internal class Util
    {
        internal static Dictionary<ExpressionType, string> NaturalOperators = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.AndAlso, "and"},
            {ExpressionType.OrElse, "or"},
            {ExpressionType.Add, "added to"},
            {ExpressionType.AddChecked, "added to"},
            {ExpressionType.And, "binary anded with"},
            {ExpressionType.Divide, "divided by"},
            {ExpressionType.Equal, "must be equal to"},
            {ExpressionType.ExclusiveOr, "exclusive or'ed with"},
            {ExpressionType.GreaterThan, "must be greater than"},
            {ExpressionType.GreaterThanOrEqual, "must be greater than or equal to"},
            {ExpressionType.LeftShift, "left shift by"},
            {ExpressionType.LessThan, "must be less than"},
            {ExpressionType.LessThanOrEqual, "must be less than or equal to"},
            {ExpressionType.Modulo, "modulo"},
            {ExpressionType.Multiply, "multiplied by"},
            {ExpressionType.MultiplyChecked, "multiplied by"},
            {ExpressionType.NotEqual, "must not be equal to"},
            {ExpressionType.Or, "binary or'ed with"},
            {ExpressionType.RightShift, "right shift by"},
            {ExpressionType.Subtract, "subtracted by"},
            {ExpressionType.SubtractChecked, "subtracted by"},
        };
        internal static Dictionary<ExpressionType, string> Operators = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.AndAlso, "&&"},
            {ExpressionType.OrElse, "||"},
            {ExpressionType.Add, "+"},
            {ExpressionType.AddChecked, "+"},
            {ExpressionType.And, "&"},
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "=="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LeftShift, "<<"},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.MultiplyChecked, "*"},
            {ExpressionType.NotEqual, "!="},
            {ExpressionType.Or, "|"},
            {ExpressionType.RightShift, ">>"},
            {ExpressionType.Subtract, "-"},
            {ExpressionType.SubtractChecked, "-"},
        };
    }
}