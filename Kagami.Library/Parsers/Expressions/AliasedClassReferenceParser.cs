using System.Text.RegularExpressions;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions;

public partial class AliasedClassReferenceParser : SymbolParser
{
   public AliasedClassReferenceParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(int|float|string|char|bytes|byte|complex|rational|long|date|number|mstring|lambda|bool|decimal|tuple|array|set|dict|optional|monad|result|lazy|event)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var alias = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      return
         from className in ParserFunctions.getClassNameFromAlias(alias)
         from classReferenceAdded in ClassReferenceParser.AddClassReference(state, builder, className)
         select classReferenceAdded;
   }
}