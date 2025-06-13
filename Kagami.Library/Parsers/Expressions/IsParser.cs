using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IsParser : SymbolParser
{
   public IsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(is)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

      var _result = getExpression(state, builder.Flags | ExpressionFlags.Comparisand);
      if (_result is (true, var comparisand))
      {
         builder.Add(new IsSymbol(comparisand));
         state.CommitTransaction();

         return unit;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}