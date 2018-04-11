using EventMonitor.Core.Triggers.Expressions;
using System;
using Xunit;

namespace EventMonitor.Monitoring.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void ValueExpressionToStringJustDisplaysValue()
        {
            var expr = new ValueExpression { Value = 15.0F };
            Assert.Equal("15", expr.ToString());
        }

        [Fact]
        public void TwoValueExpressionsWithSameValueAreEquals()
        {
            var expr = new ValueExpression { Value = 15.0F };
            var expr2 = new ValueExpression { Value = 15.0F };
            Assert.Equal(expr, expr2);
        }

        [Fact]
        public void NamePatternExpressionToStringJustDisplaysName()
        {
            var expr = new NamePatternExpression { Name = "event.name" };
            Assert.Equal("event.name", expr.ToString());
        }

        [Fact]
        public void TwoNamePatternExpressionsWithSamePatternAreEquals()
        {
            var expr = new NamePatternExpression { Name = "event.name" };
            var expr2 = new NamePatternExpression { Name = "event.name" };
            Assert.Equal(expr, expr2);
        }

        [Fact]
        public void BinaryExpressionWithValuesProducesCorrectString()
        {
            var expr = new ValueExpression { Value = 15.1F };
            var expr2 = new ValueExpression { Value = 15.5F };
            var bin = new BinaryExpression
            {
                Left = expr,
                BinaryOperator = BinaryOperator.LessThan,
                Right = expr2
            };

            Assert.Equal("15.1 < 15.5", bin.ToString());
        }

        [Fact]
        public void NestedExpressionProducesCorrectString()
        {
            var expr = new BinaryExpression
            {
                Left = new BinaryExpression
                {
                    Left = new ValueExpression { Value = 15.1F },
                    BinaryOperator = BinaryOperator.LessThan,
                    Right = new ValueExpression { Value = 15.5F }
                },
                BinaryOperator = BinaryOperator.And,
                Right = new BinaryExpression
                {
                    Left = new ValueExpression { Value = 3.0 },
                    BinaryOperator = BinaryOperator.GreaterThan,
                    Right = new ValueExpression { Value = 2.0 }
                }
            };

            Assert.Equal("15.1 < 15.5 && 3 > 2", expr.ToString());
        }
    }
}
