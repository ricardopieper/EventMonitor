using EventMonitor.Monitoring.Triggers.Expressions;
using System;
using Xunit;
using EventMonitor.Monitoring.Triggers.Expressions.Dsl;
using static EventMonitor.Monitoring.Triggers.Expressions.Dsl.TriggerExpression;

namespace EventMonitor.Monitoring.Tests
{
    public class ExpressionParserTests
    {
        [Fact]
        public void ParsesSimpleExpression()
        {
            ExpressionParser parser = new ExpressionParser();
            Expression parsed = parser.Parse("1 > 2");
            Assert.Equal((Expr.Of(1) > 2).ToString(), parsed.ToString());
        }

        [Fact]
        public void ParsesExpressionWithAnd()
        {
            ExpressionParser parser = new ExpressionParser();
            Expression parsed = parser.Parse("1 > 2 && 4 > 3");
            Assert.Equal( (Expr.Of(1) > 2 & Expr.Of(4) > 3).ToString(), parsed.ToString());
        }

        [Fact]
        public void ParsesExpressionWithFunctionCall()
        {
            ExpressionParser parser = new ExpressionParser();
            Expression parsed = parser.Parse("avg(event.name) > 2 && 4 > 3");
            Assert.Equal((new FunctionExpression { EventName = "event.name", FunctionName = "avg" } > 2 & Expr.Of(4) > 3).ToString(), parsed.ToString());
        }
    }
}
