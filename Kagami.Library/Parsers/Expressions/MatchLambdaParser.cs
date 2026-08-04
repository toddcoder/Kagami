using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MatchLambdaParser : SymbolParser
{
   [GeneratedRegex(@"^([ \t]*)(\|)(\()")]
   public override partial Regex Regex();

   public MatchLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.OpenParenthesis);

      var _result =
         from c in getExpression(state, builder.Flags | ExpressionFlags.Comparisand)
         from scanned in state.Scan(@"^(\))(\s*)(->)", Color.CloseParenthesis, Color.Whitespace, Color.Lambda)
         from typeConstraint in parseTypeConstraint(state)
         from b in getLambdaBlock(!state.CurrentSource.IsMatch("^ /s* '{'"), scanned.EndsWith("=>"), state, builder.Flags | ExpressionFlags.InLambda, typeConstraint.Maybe)
         select (c, b);

      if (_result is (true, var (comparisand, block)))
      {
         var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         expressionBuilder.Add(new FieldSymbol("__$0"));
         expressionBuilder.Add(new MatchSymbol(false));
         expressionBuilder.Add(comparisand);

         var _comparison = expressionBuilder.ToExpression();
         if (_comparison is (true, var comparison))
         {
            List<Statement> list = [new If(comparison, block)];
            var lambdaSymbol = new LambdaSymbol(1, [with(list)]);
            builder.Add(lambdaSymbol);
         }
         else
         {
            return _comparison.Exception;
         }
      }

      return unit;
   }
}