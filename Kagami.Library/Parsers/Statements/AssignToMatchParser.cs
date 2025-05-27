using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignToMatchParser : EndingInExpressionParser
{
   protected Symbol comparisand = new EmptySymbol();

   //public override string Pattern => "^ /'set' /(/s+)";

   [GeneratedRegex(@"^(set)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword, Color.Whitespace);

      var _result =
         from comparisandValue in getValue(state, ExpressionFlags.Comparisand)
         from scanned in state.Scan("^ /(/s*) /'='", Color.Whitespace, Color.Structure)
         select comparisandValue;
      if (_result is (true, var symbol))
      {
         comparisand = symbol;
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      state.AddStatement(new AssignToMatch(comparisand, expression));
      return unit;
   }
}