using EventMonitor.Monitoring.Triggers.Expressions;
using System;
using Xunit;

namespace EventMonitor.Monitoring.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void ValueExpressionToStringJustDisplaysValue()
        {
            var expr = Expr.Of(15);
            Assert.Equal("15", expr.ToString());
        }

        [Fact]
        public void TwoValueExpressionsWithSameValueAreEquals()
        {
            var expr = Expr.Of(15);
            var expr2 = Expr.Of(15);
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
            Assert.Equal("15.1 < 15.5", Expr.Of(15.1F).Lt(15.5F).ToString());
        }

        [Fact]
        public void FunctionCallProducesCorrectString()
        {
            Assert.Equal("avg(event.name) == 15.5", Expr.Avg("event.name").Eq(15.5F).ToString());
        }

        [Fact]
        public void NestedExpressionProducesCorrectString()
        {
            Assert.Equal("(15.1 < 15.5 && 3 > 2)",
                Expr.Of(15.1F).Lt(15.5).And(
                    Expr.Of(3).Gt(2)).ToString());
        }

        [Fact]
        public void MultipleNestedExpressions()
        {
            Assert.Equal("(15.1 < 15.5 || (2 < 3 && avg(event.name) < 6))",
                Expr.Of(15.1F).Lt(15.5).Or(
                    Expr.Of(2).Lt(3).And(
                        Expr.Avg("event.name").Lt(6)
                    )
                ).ToString());
        }
    }
}
