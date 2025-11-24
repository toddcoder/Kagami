using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class CastParser : SymbolParser
{
   public CastParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(/)(?={REGEX_CLASS_OR_ALIAS})")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var _possibleClassName = parseTypeConstraint(state);
      if (_possibleClassName is (true, var possibleTypeConstraint))
      {
         if (possibleTypeConstraint.Maybe is (true, var typeConstraint))
         {
            builder.Add(new AsSymbol(typeConstraint.Comparisands[0].Name));
            state.CommitTransaction();

            return unit;
         }
         else
         {
            state.RollBackTransaction();
            return nil;
         }
      }
      else
      {
         state.RollBackTransaction();
         return _possibleClassName.Exception;
      }
   }
}