using System.Text;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ComprehensionParser : SymbolParser
{
   public ComprehensionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s+)(for)(?![>\^])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _expression = builder.ToExpression();
      if (_expression is (true, var expression))
      {
         var comprehensions = new List<(Symbol, Expression, PossibleExpression, string)>();

         var _innerComprehension = getInnerComprehension(state);
         if (_innerComprehension is (true, var (comparisand, innerSource, possibleExpression)))
         {
            var image = $"for {comparisand} in {innerSource}";
            comprehensions.Add((comparisand, innerSource, possibleExpression, image));
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

         var stack = new Stack<(Symbol, Expression, PossibleExpression, string)>();
         foreach (var item in comprehensions)
         {
            stack.Push(item);
         }

         For forStatement;
         var images = new StringBuilder();
         if (stack.Count > 0)
         {
            var (symbol, source, expression1, image) = stack.Pop();
            images.Append(image);
            var yieldStatement = new Yield(expression);
            var block = new Block(yieldStatement);
            if (expression1.Maybe is (true, var boolean))
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
            var (symbol, source, expression2, image) = stack.Pop();
            images.Append(image);
            var block = new Block(forStatement);
            if (expression2.Maybe is (true, var boolean))
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