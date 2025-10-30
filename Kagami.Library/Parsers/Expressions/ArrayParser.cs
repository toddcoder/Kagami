using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ArrayParser : SymbolParser
{
   public ArrayParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\[)(?![:\.])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Collection);

      var _expression = getExpression(state, @"^(\s*)(?<![:\.])(\])", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection);
      if (_expression is (true, var expression))
      {
         var _parsedTypeConstraint = parseTypeConstraint(state);
         if (_parsedTypeConstraint is (true, var possibleTypeConstraint))
         {
            builder.Add(new ArraySymbol(expression, possibleTypeConstraint.Maybe));
            return unit;
         }
         else
         {
            return _parsedTypeConstraint.Exception;
         }
      }
      else
      {
         return _expression.Exception;
      }
   }
}