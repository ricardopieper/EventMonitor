using EventMonitor.Monitoring.Triggers.Expressions.Dsl;
using static EventMonitor.Monitoring.Triggers.Expressions.Dsl.TriggerExpression;

namespace EventMonitor.Monitoring.Triggers.Expressions
{
#pragma warning disable CS0660 // O tipo define os operadores == ou !=, mas não substitui o Object.Equals(object o)
#pragma warning disable CS0661 // O tipo define os operadores == ou !=, mas não substitui o Object.GetHashCode()
    public abstract class Expression
#pragma warning restore CS0661 // O tipo define os operadores == ou !=, mas não substitui o Object.GetHashCode()
#pragma warning restore CS0660 // O tipo define os operadores == ou !=, mas não substitui o Object.Equals(object o)
    {
        public static TriggerExpression operator |(Expression left, Expression right) => New(left.And(right));
        public static TriggerExpression operator &(Expression left, Expression right) => New(left.And(right));
        public static TriggerExpression operator >(Expression left, Expression right) => New(left.Gt(right));
        public static TriggerExpression operator <(Expression left, Expression right) => New(left.Lt(right));
        public static TriggerExpression operator >=(Expression left, Expression right) => New(left.Gte(right));
        public static TriggerExpression operator <=(Expression left, Expression right) => New(left.Lte(right));
        public static TriggerExpression operator ==(Expression left, Expression right) => New(left.Eq(right));
        public static TriggerExpression operator !=(Expression left, Expression right) => New(left.NEq(right));

        public static TriggerExpression operator >(Expression left, object right) => New(left.Gt(right));
        public static TriggerExpression operator <(Expression left, object right) => New(left.Lt(right));
        public static TriggerExpression operator >=(Expression left, object right) => New(left.Gte(right));
        public static TriggerExpression operator <=(Expression left, object right) => New(left.Lte(right));
        public static TriggerExpression operator ==(Expression left, object right) => New(left.Eq(right));
        public static TriggerExpression operator !=(Expression left, object right) => New(left.NEq(right));

        public static TriggerExpression operator >(object left, Expression right) => New(Expr.Of(left).Gt(right));
        public static TriggerExpression operator <(object left, Expression right) => New(Expr.Of(left).Lt(right));
        public static TriggerExpression operator >=(object left, Expression right) => New(Expr.Of(left).Gte(right));
        public static TriggerExpression operator <=(object left, Expression right) => New(Expr.Of(left).Lte(right));
        public static TriggerExpression operator ==(object left, Expression right) => New(Expr.Of(left).Eq(right));
        public static TriggerExpression operator !=(object left, Expression right) => New(Expr.Of(left).NEq(right));
    }
}
