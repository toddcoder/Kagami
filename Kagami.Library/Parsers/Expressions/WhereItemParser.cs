using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhereItemParser : EndingInExpressionParser
{
   public WhereItemParser(ExpressionBuilder builder) :
      base(builder, ExpressionFlags.OmitColon | ExpressionFlags.OmitComma | ExpressionFlags.Comparisand)
   {
   }

   protected string propertyName = "";

   //public override string Pattern => $"^ /(/s*) /({REGEX_FIELD}) /(/s*) /':'";

   [GeneratedRegex(@$"^(\s*)({REGEX_FIELD})(\s*)(:)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      propertyName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Whitespace, Color.Structure);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      Expression = expression;
      return unit;
   }

   public string PropertyName => propertyName;

   public Expression Expression { get; set; } = Expression.Empty;
}