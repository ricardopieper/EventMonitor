using System;
using System.Collections.Generic;

namespace EventMonitor.Core.Triggers.Expression
{
    public class BinaryExpression : Expression
    {
        public Expression Left { get; set; }
        public BinaryOperator BinaryOperator { get; set; }
        public Expression Right { get; set; }

        public override bool Equals(object obj)
        {
            var expression = obj as BinaryExpression;
            return expression != null &&
                   EqualityComparer<Expression>.Default.Equals(Left, expression.Left) &&
                   BinaryOperator == expression.BinaryOperator &&
                   EqualityComparer<Expression>.Default.Equals(Right, expression.Right);
        }

        public override int GetHashCode()
        {
            var hashCode = 661756193;
            hashCode = hashCode * -1521134295 + EqualityComparer<Expression>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + BinaryOperator.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Expression>.Default.GetHashCode(Right);
            return hashCode;
        }

        public override string ToString() => $"{Left.ToString()} {GetOperator()} {Right.ToString()}";

        public static bool operator ==(BinaryExpression expression1, BinaryExpression expression2) => EqualityComparer<BinaryExpression>.Default.Equals(expression1, expression2);

        public static bool operator !=(BinaryExpression expression1, BinaryExpression expression2) => !(expression1 == expression2);

        private string GetOperator()
        {
            switch (BinaryOperator)
            {
                case BinaryOperator.And: return "&&";
                case BinaryOperator.Equals: return "==";
                case BinaryOperator.GreaterOrEquals: return ">=";
                case BinaryOperator.GreaterThan: return ">";
                case BinaryOperator.LessOrEquals: return "<=";
                case BinaryOperator.LessThan: return "<";
                case BinaryOperator.Minus: return "-";
                case BinaryOperator.Multiply: return "*";
                case BinaryOperator.Divide: return "/";
                case BinaryOperator.Plus: return "+";
                //@TODO ExpressionException
                default: throw new Exception("Operator not implemented: " + BinaryOperator);
            }
        }

    }
}
