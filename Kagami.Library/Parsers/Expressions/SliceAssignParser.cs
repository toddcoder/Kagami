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

   //public override string Pattern => "^ /'{'";

   [GeneratedRegex(@"^({)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();
      state.Colorize(tokens, Color.Structure);

      var _symbol =
         from indexes in getExpression(state, @"^(\s*)(\})(\s*)(=)", builder.Flags, Color.Whitespace, Color.Structure,
            Color.Whitespace, Color.Structure)
         from values in getExpression(state, builder.Flags)
         select new SliceAssignSymbol(indexes, values);
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