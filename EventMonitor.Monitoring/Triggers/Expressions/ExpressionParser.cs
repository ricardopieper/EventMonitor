using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EventMonitor.Monitoring.Triggers.Expressions
{
    /// <summary>
    /// Partial implementation of https://www.geeksforgeeks.org/stack-set-2-infix-to-postfix/
    /// </summary>
    public class ExpressionParser
    {
        public static readonly String[] AllowedFunctions = new[] { "avg", "min", "max" };

        private Dictionary<object, int> PrecedenceTable = new Dictionary<object, int>
        {
            {BinaryOperator.GreaterOrEquals, 3},
            {BinaryOperator.GreaterThan, 3},
            {BinaryOperator.LessOrEquals, 3},
            {BinaryOperator.LessThan, 3},
            {BinaryOperator.Divide, 2},
            {BinaryOperator.Multiply, 2},
            {BinaryOperator.Plus, 1},
            {BinaryOperator.Minus, 1},
            {BinaryOperator.And, 0},
            {BinaryOperator.Or, 0}
        };

        public Expression Parse(string str)
        {
            try
            {
                ParseState ps = new ParseState(str);

                List<object> fragments = new List<object>();

                while (!ps.IsEof())
                {
                    SkipWhitespace(ps);
                    char c = ps.Peek();
                    if (char.IsLetter(c))
                    {
                        FunctionExpression expression = ParseFunctionExpression(ps);
                        fragments.Add(expression);
                    }
                    else if (IsOperator(c))
                    {
                        BinaryOperator @operator = GetBinaryOperator(ps);
                        fragments.Add(@operator);
                    }
                    else
                    {
                        String literal = GetLiteral(ps);
                        if (Decimal.TryParse(literal, out var v))
                        {
                            fragments.Add(new LiteralValueExpression { Value = v });
                        }
                        else
                        {
                            fragments.Add(new NamePatternExpression { Name = literal });
                        }
                    }
                    ps.PeekAndAdvance();
                }

                Stack<BinaryOperator> stack = new Stack<BinaryOperator>();
                List<object> output = new List<object>();
                foreach (object o in fragments)
                {
                    if (o is Expression expr)
                    {
                        output.Add(o as Expression);
                    }
                    else if (o is BinaryOperator op)
                    {
                        if (stack.Count == 0 || PrecedenceTable[op] > PrecedenceTable[stack.Peek()])
                        {
                            stack.Push(op);
                        }
                        else
                        {
                            while (stack.TryPop(out BinaryOperator result)
                                && PrecedenceTable[op] <= PrecedenceTable[result])
                            {
                                output.Add(result);
                            }
                            stack.Push(op);
                        }
                    }
                }

                while (stack.TryPop(out BinaryOperator result))
                {
                    output.Add(result);
                }

                return BuildExpression(output);

            }
            catch (Exception ex)
            {
                if (ex is ExpressionParseException) throw;
                throw new ExpressionParseException("Wrong expression couldn't be parsed, but error is unknown: " + ex.Message);
            }
        }

        private Expression BuildExpression(List<object> postfixExpr)
        {
            Stack<Expression> expr = new Stack<Expression>();
            foreach (object o in postfixExpr)
            {
                if (o is BinaryOperator binOp)
                {
                    Expression right = expr.Pop();
                    Expression left = expr.Pop();
                    expr.Push(new BinaryExpression
                    {
                        Left = left,
                        Right = right,
                        BinaryOperator = binOp
                    });
                }
                else
                {
                    expr.Push(o as Expression);
                }
            }
            if (expr.Count == 1)
            {
                return expr.Pop();
            }
            else
            {
                throw new ExpressionParseException("Failed to parse expr: extra tokens found " + string.Join(",", expr.Reverse().ToList()));
            }
        }

        private BinaryOperator GetBinaryOperator(ParseState ps)
        {
            char c = ps.Peek();
            if (c == '=')
            {
                return TryGetEquals(ps);
            }
            if (c == '!')
            {
                return TryGetNotEquals(ps);
            }
            else if (c == '>')
            {
                return TryGetGreaterThan(ps);
            }
            else if (c == '<')
            {
                return TryGetLessThan(ps);
            }
            else if (c == '&')
            {
                return TryGetAnd(ps);
            }
            else if (c == '|')
            {
                return TryGetOr(ps);
            }
            else if (c == '+')
            {
                return BinaryOperator.Plus;
            }
            else if (c == '-')
            {
                return BinaryOperator.Minus;
            }
            else if (c == '*')
            {
                return BinaryOperator.Multiply;
            }
            else if (c == '/')
            {
                return BinaryOperator.Divide;
            }
            else throw new ExpressionParseException("Failed to get operator at position " + ps.CurrentPosition + ", got " + c);
        }

        private static BinaryOperator TryGetOr(ParseState ps)
        {
            if (ps.Lookahead(1) == '|')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.Or;
            }
            else throw new ExpressionParseException("Expected ||, got |" + ps.Lookahead(1) + " at position " + ps.CurrentPosition);
        }
        private static BinaryOperator TryGetAnd(ParseState ps)
        {
            if (ps.Lookahead(1) == '&')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.And;
            }
            else throw new ExpressionParseException("Expected &&, got &" + ps.Lookahead(1) + " at position " + ps.CurrentPosition);
        }

        private static BinaryOperator TryGetLessThan(ParseState ps)
        {
            if (ps.Lookahead(1) == '=')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.LessOrEquals;
            }
            else return BinaryOperator.LessThan;
        }

        private static BinaryOperator TryGetGreaterThan(ParseState ps)
        {
            if (ps.Lookahead(1) == '=')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.GreaterOrEquals;
            }
            else return BinaryOperator.GreaterThan;
        }

        private static BinaryOperator TryGetEquals(ParseState ps)
        {
            if (ps.Lookahead(1) == '=')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.Equals;
            }
            else throw new ExpressionParseException("Expected ==, got =" + ps.Lookahead(1) + " at position " + ps.CurrentPosition);
        }
        private static BinaryOperator TryGetNotEquals(ParseState ps)
        {
            if (ps.Lookahead(1) == '=')
            {
                ps.PeekAndAdvance();
                return BinaryOperator.Equals;
            }
            else throw new ExpressionParseException("Expected !=, got !" + ps.Lookahead(1) + " at position " + ps.CurrentPosition);
        }

        private bool IsOperator(char c)
        {
            return c == '=' || c == '>' || c == '<' || c == '&' || c == '!';
        }

        private FunctionExpression ParseFunctionExpression(ParseState ps)
        {
            String function = GetFunctionCall(ps);
            ExpectOpenPar(ps);
            ps.PeekAndAdvance();
            String literal = GetLiteral(ps);
            ExpectClosingPar(ps);
            return new FunctionExpression
            {
                FunctionName = function,
                EventName = literal
            };
        }

        private string GetFunctionCall(ParseState ps)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (!ps.IsEof() && ps.Peek() != '(')
            {
                stringBuilder.Append(ps.Peek());
                ps.PeekAndAdvance();
            }
            String str = stringBuilder.ToString();
            if (!AllowedFunctions.Contains(str))
            {
                throw new ExpressionParseException("Function " + str + " is not defined");
            }
            return str;
        }

        private void ExpectOpenPar(ParseState ps)
        {
            if (ps.Peek() != '(')
            {
                throw new ExpressionParseException("Expected opening parenthesis at position " + ps.CurrentPosition);
            }
        }

        private void ExpectClosingPar(ParseState ps)
        {
            if (ps.Peek() != ')')
            {
                throw new ExpressionParseException("Expected closing parenthesis at position " + ps.CurrentPosition);
            }
        }

        private void SkipWhitespace(ParseState ps)
        {
            while (!ps.IsEof() && IsWhitespace(ps.Peek()))
            {
                ps.PeekAndAdvance();
            }
        }

        private static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private String GetLiteral(ParseState ps)
        {
            StringBuilder sb = new StringBuilder();
            while (!ps.IsEof() && (char.IsLetterOrDigit(ps.Peek()) || ps.Peek() == '_' || ps.Peek() == '.'))
            {
                sb.Append(ps.PeekAndAdvance());
            }
            return sb.ToString();
        }
    }

    public class ExpressionParseException : Exception
    {
        public ExpressionParseException() { }
        public ExpressionParseException(string message) : base(message) { }
        public ExpressionParseException(string message, Exception inner) : base(message, inner) { }
        protected ExpressionParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    class ParseState
    {
        public ParseState(string s)
        {
            Source = s;
        }

        public int CurrentPosition { get; private set; } = 0;
        public string Source { get; }
        public char Peek() => IsEof() ? char.MinValue : Source[CurrentPosition];
        public char Lookahead(int i) => Source[CurrentPosition + i];
        public char PeekAndAdvance()
        {
            char c = Peek();
            if (!IsEof()) CurrentPosition++;
            return c;
        }
        public bool IsEof() => CurrentPosition >= Source.Length;
    }
}
