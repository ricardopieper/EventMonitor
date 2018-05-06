using System;
using System.Collections.Generic;
using System.Text;

namespace EventMonitor.Monitoring.Triggers.Expressions
{
    public static class Expr
    {
        public static FunctionExpression Avg(string eventName) => new FunctionExpression { EventName = eventName, FunctionName = "avg" };
        public static FunctionExpression Max(string eventName) => new FunctionExpression { EventName = eventName, FunctionName = "max" };
        public static FunctionExpression Min(string eventName) => new FunctionExpression { EventName = eventName, FunctionName = "min" };
        public static FunctionExpression Sum(string eventName) => new FunctionExpression { EventName = eventName, FunctionName = "sum" };

        public static LiteralValueExpression Of(Object value) => new LiteralValueExpression { Value = value };

        public static BinaryExpression And(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.And };
        public static BinaryExpression Or(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.Or };
        public static BinaryExpression Gte(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.GreaterOrEquals };
        public static BinaryExpression Gt(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.GreaterThan };
        public static BinaryExpression Lte(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.LessOrEquals };
        public static BinaryExpression Lt(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.LessThan };
        public static BinaryExpression Eq(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.Equals };
        public static BinaryExpression NEq(this Expression left, Expression right) => new BinaryExpression { Left = left, Right = right, BinaryOperator = BinaryOperator.Equals };

        public static BinaryExpression Gte(this Expression left, Object right) => left.Gte(Of(right));
        public static BinaryExpression Gt(this Expression left, Object right) => left.Gt(Of(right));
        public static BinaryExpression Lte(this Expression left, Object right) => left.Lte(Of(right));
        public static BinaryExpression Lt(this Expression left, Object right) => left.Lt(Of(right));
        public static BinaryExpression Eq(this Expression left, Object right) => left.Eq(Of(right));
        public static BinaryExpression NEq(this Expression left, Object right) => left.NEq(Of(right));

     

    }
}
