using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class MatchLambdaParser : SymbolParser
{
   //public override string Pattern => "^ /(/s*) /'|('";

   [GeneratedRegex(@"^(\s*)(\|\()")]
   public override partial Regex Regex();

   public MatchLambdaParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);

      var _result =
         from c in getExpression(state, builder.Flags | ExpressionFlags.Comparisand)
         from scanned in state.Scan(@"^(\))(\s*)(->|=>)", Color.Structure, Color.Whitespace, Color.Structure)
         from typeConstraint in parseTypeConstraint(state)
         from b in getLambdaBlock(scanned.Contains("->"), state, builder.Flags, typeConstraint.Maybe)
         select (c, b);

      if (_result is (true, var (comparisand, block)))
      {
         var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         expressionBuilder.Add(new FieldSymbol("__$0"));
         expressionBuilder.Add(new MatchSymbol());
         expressionBuilder.Add(comparisand);

         var _comparison = expressionBuilder.ToExpression();
         if (_comparison is (true, var comparison))
         {
            var list = new List<Statement> { new If(comparison, block) };
            var lambdaSymbol = new LambdaSymbol(1, new Block(list));
            builder.Add(lambdaSymbol);

            return unit;
         }
         else
         {
            return _comparison.Exception;
         }
      }

      return unit;
   }
}