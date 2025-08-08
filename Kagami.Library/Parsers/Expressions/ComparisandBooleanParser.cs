using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class ComparisandBooleanParser : SymbolParser
{
   public ComparisandBooleanParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(==|!=|<=|>=|<|>)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);
      switch (tokens[2].Text)
      {
         case "==":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Equal));
            break;
         case "!=":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Equal | SpecialComparisandDirection.Not));
            break;
         case "<=":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Less | SpecialComparisandDirection.Equal));
            break;
         case ">=":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Greater | SpecialComparisandDirection.Equal));
            break;
         case "<":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Less));
            break;
         case ">":
            builder.Add(new SpecialComparisandSymbol(SpecialComparisandDirection.Greater));
            break;
         default:
            return nil;
      }

      return unit;
   }
}