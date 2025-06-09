using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Runtime;

namespace Kagami.Library.Parsers.Statements;

public class InclusionFunctionParser(Inclusion inclusion) : FunctionParser
{
   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      return
         from statement in base.ParseStatement(state, tokens)
         from function in _function
         from result in inclusion.Register(function)
         select result;
   }
}