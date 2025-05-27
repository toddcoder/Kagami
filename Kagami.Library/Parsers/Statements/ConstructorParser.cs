using System.Text.RegularExpressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ConstructorParser : ClassItemParser
{
   public ConstructorParser(ClassBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /'init' /'('";

   [GeneratedRegex(@"^(init)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseClassItem(ParseState state, Token[] tokens, ClassBuilder builder)
   {
      state.Colorize(tokens, Color.Keyword, Color.OpenParenthesis);
      state.CreateReturnType();
      return
         from parameters in getParameters(state)
         from block in getAnyBlock(state)
         from constructor in builder.Constructor(parameters, block, false)
         select constructor;
   }
}