using Kagami.Library.Classes;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Kagami.Library.Parsers.Statements.FunctionParser;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class SignatureParser : StatementParser
   {
      TraitClass traitClass;

      public SignatureParser(TraitClass traitClass) => this.traitClass = traitClass;

      public override string Pattern => $"^ /'abstract' /(/s+) /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var functionName = tokens[3].Text;
         var needsParameters = tokens[4].Text == "(";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Invokable, Color.Structure);

         if (needsParameters)
         {
            if (functionName.EndsWith("="))
               functionName = functionName.Skip(-1).set();
         }
         else
            functionName = functionName.get();

         if (GetAnyParameters(needsParameters, state).If(out var parameters, out var original))
         {
            var fullFunctionName = parameters.FullFunctionName(functionName);
            if (traitClass.RegisterSignature(new Signature(fullFunctionName, parameters.Length)).If(out var _, out var exception))
               return Unit.Matched();
            else
               return failedMatch<Unit>(exception);
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}