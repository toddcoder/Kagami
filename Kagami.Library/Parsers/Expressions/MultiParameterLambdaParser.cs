using Kagami.Library.Invokables;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MultiParameterLambdaParser : LambdaParser
   {
      public override string Pattern => "^ /(|s|) /'('";

      public MultiParameterLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Parameters> ParseParameters(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Structure);
         return getParameters(state);
      }
   }
}