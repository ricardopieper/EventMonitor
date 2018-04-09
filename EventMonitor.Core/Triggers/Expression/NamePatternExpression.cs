using System;
using System.Collections.Generic;

namespace EventMonitor.Core.Triggers.Expression
{
    public class NamePatternExpression : Expression
    {
        public String Name { get; set; }

        public override bool Equals(object obj)
        {
            var expression = obj as NamePatternExpression;
            return expression != null &&
                   Name == expression.Name;
        }

        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);

        public static bool operator ==(NamePatternExpression expression1, NamePatternExpression expression2) => EqualityComparer<NamePatternExpression>.Default.Equals(expression1, expression2);

        public static bool operator !=(NamePatternExpression expression1, NamePatternExpression expression2) => !(expression1 == expression2);

        public override string ToString() => Name.ToString();
    }
}
