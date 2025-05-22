using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class DoParser : SymbolParser
{
   protected class BoundItemParser : EndingInExpressionParser
   {
      protected string fieldName;

      public BoundItemParser(ExpressionBuilder builder) : base(builder) => fieldName = "";

      public override string Pattern => $"^ /({REGEX_FIELD}) /(/s*) /'<-'";

      public (string, Expression) NameExpression { get; set; }

      public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
      {
         fieldName = tokens[1].Text;
         state.Colorize(tokens, Color.Identifier, Color.Whitespace, Color.Structure);

         return unit;
      }

      public override Optional<Unit> Suffix(ParseState state, Expression expression)
      {
         NameExpression = (fieldName, expression);
         return unit;
      }
   }

   public DoParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(/s*) /'do' {REGEX_ANTICIPATE_END}";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var innerBuilder = new ExpressionBuilder(builder.Flags);
      var boundItemParser = new BoundItemParser(innerBuilder);
      var stack = new Stack<(string, Expression)>();

      var _result = state.BeginBlock();
      if (_result)
      {
         while (state.More)
         {
            var _nameExpression =
               from unit in boundItemParser.Scan(state)
               select boundItemParser.NameExpression;
            if (_nameExpression is (true, var nameExpression))
            {
               stack.Push(nameExpression);
               state.SkipEndOfLine();
            }
            else if (_nameExpression.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }

         var _lambdaExpression = getExpression(state, builder.Flags);

         _result = state.EndBlock();
         if (!_result)
         {
            return _result.Exception;
         }

         if (_lambdaExpression is (true, var lambdaExpression))
         {
            var (parameterName, targetExpression) = stack.Pop();
            var _symbol = getSymbol(targetExpression, parameterName, lambdaExpression, stack);
            if (_symbol is (true, var symbol))
            {
               builder.Add(symbol);
               return unit;
            }
            else
            {
               return _symbol.Exception;
            }
         }
         else if (_lambdaExpression.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return fail("Missing 'gather' expression");
         }
      }
      else
      {
         return _result.Exception;
      }
   }

   protected static Result<Symbol> getSymbol(Expression targetExpression, string parameterName, Expression lambdaExpression,
      Stack<(string, Expression)> stack)
   {
      var block = new Block(lambdaExpression);
      var lambda = new LambdaSymbol(new Parameters(parameterName), block);
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      builder.Add(targetExpression);
      builder.Add(new SendMessageSymbol("bind(_<Lambda>)", lambda));

      var _expression = builder.ToExpression();
      if (_expression is (true, var expression))
      {
         if (stack.Count == 0)
         {
            return new SubexpressionSymbol(expression);
         }
         else
         {
            var (nextName, nextExpression) = stack.Pop();
            return getSymbol(nextExpression, nextName, expression, stack);
         }
      }
      else
      {
         return _expression.Exception;
      }
   }
}