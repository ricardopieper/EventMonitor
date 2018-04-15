using System;
using System.Collections.Generic;
using System.Globalization;

namespace EventMonitor.Monitoring.Triggers.Expressions
{
    public class LiteralValueExpression : Expression
    {
        public Object Value { get; set; }

        public override bool Equals(object obj)
        {
            var expression = obj as LiteralValueExpression;
            return expression != null &&
                   EqualityComparer<object>.Default.Equals(Value, expression.Value);
        }

        public override int GetHashCode() => -1937169414 + EqualityComparer<object>.Default.GetHashCode(Value);

        public static bool operator ==(LiteralValueExpression expression1, LiteralValueExpression expression2) => EqualityComparer<LiteralValueExpression>.Default.Equals(expression1, expression2);

        public static bool operator !=(LiteralValueExpression expression1, LiteralValueExpression expression2) => !(expression1 == expression2);

        public override string ToString()
        {
            return Value is IConvertible v ? v.ToString(CultureInfo.InvariantCulture)
              : Value is IFormattable f ? f.ToString(null, CultureInfo.InvariantCulture)
              : Value.ToString();
        }
    }
}
