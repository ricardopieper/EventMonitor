using EventMonitor.Monitoring.Triggers.Expressions;
using System;
using Xunit;
using EventMonitor.Monitoring.Triggers.Expressions.Dsl;
using static EventMonitor.Monitoring.Triggers.Expressions.Dsl.TriggerExpression;

namespace EventMonitor.Monitoring.Tests
{
    public class DslTests
    {
        [Fact]
        public void BinaryExpressionWithValuesProducesCorrectString()
        {
            Assert.Equal("15.1 < 15.5", (Expr.Of(15.1F) < 15.5F).ToString());
        }

        [Fact]
        public void FunctionCallProducesCorrectString()
        {
            Assert.Equal("avg(event.name) == 15.5", (Expr.Avg("event.name") == 15.5F).ToString());
        }

        [Fact]
        public void NestedExpressionProducesCorrectString()
        {
            Assert.Equal("(15.1 < 15.5 && 3 > 2)",
                ((Expr.Of(15.1F) < 15.5) & (Expr.Of(3) > 2)).ToString());
        }

        [Fact]
        public void MultipleNestedExpressions()
        {
            Assert.Equal("(15.1 < 15.5 || (2 < 3 && avg(event.name) < 6))",
                (Expr.Of(15.1F) < 15.5 | (
                    Expr.Of(2) < 3 & (
                        Expr.Avg("event.name") < 6
                    )
                )).ToString());
        }
    }
}
