using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InnerComprehensionParser : SymbolParser
{
   protected List<(Symbol, Expression, Maybe<Expression>, string)> comprehensions;

   public InnerComprehensionParser(ExpressionBuilder builder, List<(Symbol, Expression, Maybe<Expression>, string)> comprehensions) :
      base(builder)
   {
      this.comprehensions = comprehensions;
   }

   //public override string Pattern => "^ /(/s*) /'for' -(> ['^>']) /b";

   [GeneratedRegex(@"^(\s*)(for)(?![\^>])\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _innerComprehension = getInnerComprehension(state);
      if (_innerComprehension is (true, var (comparisand, source, ifExp)))
      {
         var image = $"for {comparisand} := {source}";
         comprehensions.Add((comparisand, source, ifExp, image));

         return unit;
      }
      else
      {
         return _innerComprehension.Exception;
      }
   }
}