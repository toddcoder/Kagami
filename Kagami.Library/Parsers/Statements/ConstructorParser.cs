using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ConstructorParser : ClassItemParser
   {
      public ConstructorParser(ClassBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'init' /'('";

      public override IMatched<Unit> ParseClassItem(ParseState state, Token[] tokens, ClassBuilder builder)
      {
         state.Colorize(tokens, Color.Keyword, Color.Structure);
	      return
		      from parameters in getParameters(state)
		      from block in getAnyBlock(state)
            from constructor in builder.Constructor(parameters, block, false)
            select constructor;
      }
   }
}