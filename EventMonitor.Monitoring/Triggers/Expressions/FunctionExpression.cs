using System;
using System.Collections.Generic;
using System.Globalization;

namespace EventMonitor.Monitoring.Triggers.Expressions
{
    public class FunctionExpression : Expression
    {
        public String FunctionName { get; set; }
        public String EventName { get; set; }

        public override bool Equals(object obj)
        {
            var f = obj as FunctionExpression;
            return f != null &&
                   f.FunctionName == FunctionName
                   && f.EventName == EventName;
        }
       
        public static bool operator ==(FunctionExpression expression1, FunctionExpression expression2) => EqualityComparer<FunctionExpression>.Default.Equals(expression1, expression2);
        public static bool operator !=(FunctionExpression expression1, FunctionExpression expression2) => !(expression1 == expression2);

        public override string ToString() => $"{FunctionName}({EventName})";

        public override int GetHashCode()
        {
            var hashCode = -1365808498;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FunctionName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EventName);
            return hashCode;
        }
    }
}
