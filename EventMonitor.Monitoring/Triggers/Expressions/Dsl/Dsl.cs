using System;
using System.Collections.Generic;
using System.Text;

namespace EventMonitor.Monitoring.Triggers.Expressions.Dsl
{
#pragma warning disable CS0660 
#pragma warning disable CS0661
    public class TriggerExpression
#pragma warning restore CS0661
#pragma warning restore CS0660
    {
        public Expression Expression { get; set; }
        public static TriggerExpression New(Expression expression) => new TriggerExpression
        {
            Expression = expression
        };

        public override string ToString()
        {
            return Expression.ToString();
        }

        public static implicit operator TriggerExpression(Expression expr) => New(expr);
        public static implicit operator Expression(TriggerExpression expr) => expr.Expression;

        public static TriggerExpression operator &(TriggerExpression left, TriggerExpression right) => New(left.Expression.And(right.Expression));
        public static TriggerExpression operator |(TriggerExpression left, TriggerExpression right) => New(left.Expression.Or(right.Expression));
        public static TriggerExpression operator >(TriggerExpression left, TriggerExpression right) => New(left.Expression.Gt(right.Expression));
        public static TriggerExpression operator <(TriggerExpression left, TriggerExpression right) => New(left.Expression.Lt(right.Expression));
        public static TriggerExpression operator >=(TriggerExpression left, TriggerExpression right) => New(left.Expression.Gte(right.Expression));
        public static TriggerExpression operator <=(TriggerExpression left, TriggerExpression right) => New(left.Expression.Lte(right.Expression));
        public static TriggerExpression operator ==(TriggerExpression left, TriggerExpression right) => New(left.Expression.Eq(right.Expression));
        public static TriggerExpression operator !=(TriggerExpression left, TriggerExpression right) => New(left.Expression.NEq(right.Expression));
    }
}






