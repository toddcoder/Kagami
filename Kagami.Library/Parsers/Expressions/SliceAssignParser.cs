using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SliceAssignParser : SymbolParser
{
   public SliceAssignParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex("^({)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.OpenParenthesis);

      var _symbol =
         from skipTake in getSkipTake(state, builder.Flags | ExpressionFlags.OmitColon)
         from scan in state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure)
         from expression in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
         select new SliceAssignSymbol(skipTake, expression);
      if (_symbol is (true, var symbol))
      {
         builder.Add(symbol);
         state.CommitTransaction();

         return unit;
      }
      else if (_symbol.Exception is (true, var exception))
      {
         state.RollBackTransaction();
         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }
}