using Core.Monads;
using Kagami.Library.Inclusions;

namespace Kagami.Library.Parsers.Statements;

public class InclusionFunctionParser(Inclusion inclusion) : FunctionParser
{
   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var _statement = base.ParseStatement(state, tokens);
      if (_statement)
      {

      }
      return _statement;
   }
}