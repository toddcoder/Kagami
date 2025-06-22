using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SetPropertyParser(ExpressionBuilder builder, string tempObjectField, ExpressionBuilder outerBuilder)
   : EndingInExpressionParser(builder)
{
   protected string tempObjectField = tempObjectField;
   protected ExpressionBuilder outerBuilder = outerBuilder;
   protected string propertyName = "";


   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(\s*)=")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      propertyName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Whitespace, Color.Structure);
      state.SkipEndOfLine();

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      outerBuilder.Add(new FieldSymbol(tempObjectField));
      Maybe<LambdaSymbol> _lambdaSymbol = nil;
      outerBuilder.Add(new SendMessageSymbol(propertyName.set(), _lambdaSymbol, expression));

      return unit;
   }
}