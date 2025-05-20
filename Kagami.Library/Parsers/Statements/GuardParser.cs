using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class GuardParser : EndingInExpressionParser
{
   public override string Pattern => "^ /'guard' /b";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);
      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      var block = new Block(new Pass());
      var _elseBlock =
         from keyword in state.Scan("^ /(|s+|) /'else'", Color.Whitespace, Color.Keyword)
         from eBlock in getBlock(state)
         select eBlock;
      if (_elseBlock is (true, var elseBlock))
      {
         state.AddStatement(new If(expression, block, nil, elseBlock.Some(), "", false, false, true));
         return unit;
      }
      else
      {
         return _elseBlock.Exception;
      }
   }
}