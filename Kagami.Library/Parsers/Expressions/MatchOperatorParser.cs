using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MatchOperatorParser : SymbolParser
{
   public MatchOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(match)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      state.PushStatements();

      var _beginBlock = state.BeginBlock();
      if (!_beginBlock)
      {
         return _beginBlock.Exception;
      }

      while (state.More)
      {
         var _endBlock = state.EndBlock();
         if (_endBlock)
         {
            break;
         }
         else if (_endBlock.Exception is (true, var exception))
         {
            return exception;
         }


      }

      return unit;
   }
}