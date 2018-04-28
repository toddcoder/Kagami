using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Statements
{
   public abstract class ClassItemParser : StatementParser
   {
      protected ClassBuilder builder;

      public ClassItemParser(ClassBuilder builder) => this.builder = builder;

      public abstract IMatched<Unit> ParseClassItem(ParseState state, Token[] tokens, ClassBuilder builder);

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         return ParseClassItem(state, tokens, builder);
      }
   }
}