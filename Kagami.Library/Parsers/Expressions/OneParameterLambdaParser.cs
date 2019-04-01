using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OneParameterLambdaParser : LambdaParser
   {
      public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /b (> |s| ('->' | '=>' [/r/n]+))";

      public OneParameterLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens)
      {
         var name = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);

         return new Parameters(new[] { new Parameter(false, "", name, none<IInvokable>(), none<TypeConstraint>(), false, false) })
            .Matched();
      }
   }
}