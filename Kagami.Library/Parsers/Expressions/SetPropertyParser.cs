using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SetPropertyParser : EndingInExpressionParser
   {
      protected string tempObjectField;
      protected ExpressionBuilder outerBuilder;
      protected string propertyName;

      public SetPropertyParser(ExpressionBuilder builder, string tempObjectField, ExpressionBuilder outerBuilder) : base(builder)
      {
         this.tempObjectField = tempObjectField;
         this.outerBuilder = outerBuilder;
      }

      public override string Pattern => $"^ /(/s*) /({REGEX_FIELD}) /(|s|) /'='";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         propertyName = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Whitespace, Color.Structure);
         state.SkipEndOfLine();

         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         outerBuilder.Add(new FieldSymbol(tempObjectField));
         outerBuilder.Add(new SendMessageSymbol(propertyName.set(), none<LambdaSymbol>(), expression));

         return Unit.Matched();
      }
   }
}