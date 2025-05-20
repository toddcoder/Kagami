using Core.Monads;

namespace Kagami.Library.Parsers.Statements;

public abstract class ClassItemParser : StatementParser
{
   protected ClassBuilder builder;

   public ClassItemParser(ClassBuilder builder) => this.builder = builder;

   public abstract Optional<Unit> ParseClassItem(ParseState state, Token[] tokens, ClassBuilder builder);

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      return ParseClassItem(state, tokens, builder);
   }
}