using System.Collections.Generic;
using System.Text;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class ComprehensionParser : SymbolParser
{
   public ComprehensionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => "^ /(|s|) /'for' -(> ['>^']) /b";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _expression = builder.ToExpression();
      if (_expression is (true, var expression))
      {
         var comprehensions = new List<(Symbol, Expression, Maybe<Expression>, string)>();

         var _innerComprehension = getInnerComprehension(state);
         if (_innerComprehension is (true, var (comparisand, innerSource, _innerIfExp)))
         {
            var image = $"for {comparisand} := {innerSource}";
            comprehensions.Add((comparisand, innerSource, _innerIfExp, image));
         }
         else
         {
            return _innerComprehension.Exception;
         }

         while (state.More)
         {
            var parser = new InnerComprehensionParser(builder, comprehensions);
            var _scan = parser.Scan(state);
            if (_scan)
            {
            }
            else if (_scan.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }

         var stack = new Stack<(Symbol, Expression, Maybe<Expression>, string)>();
         foreach (var item in comprehensions)
         {
            stack.Push(item);
         }

         For forStatement;
         var images = new StringBuilder();
         if (stack.Count > 0)
         {
            var (symbol, source, _ifExp, image) = stack.Pop();
            images.Append(image);
            var yieldStatement = new Yield(expression);
            var block = new Block(yieldStatement);
            if (_ifExp is (true, var boolean))
            {
               block = new Block(new If(boolean, block));
            }

            forStatement = new For(symbol, source, block);
         }
         else
         {
            return nil;
         }

         while (stack.Count > 0)
         {
            var (symbol, source, _ifExp, image) = stack.Pop();
            images.Append(image);
            var block = new Block(forStatement);
            if (_ifExp is (true, var boolean))
            {
               block = new Block(new If(boolean, block));
            }

            forStatement = new For(symbol, source, block);
         }

         builder.Clear();

         state.CreateReturnType();

         var statements = new List<Statement>
         {
            forStatement, new Return(new Expression(new NoneSymbol()), state.GetReturnType())
         };

         state.RemoveReturnType();

         builder.Add(new ComprehensionSymbol(new Block(statements), images.ToString()));

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}