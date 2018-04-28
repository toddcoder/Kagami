using Kagami.Library.Invokables;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class OneParameterLambdaParser : LambdaParser
   {
      public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /b (> |s| ('->' | '=>'))";

      public OneParameterLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens)
      {
         var name = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);

         return new Parameters(new[] { new Parameter(false, "", name, none<IInvokable>(), false), }).Matched();
      }
   }
}