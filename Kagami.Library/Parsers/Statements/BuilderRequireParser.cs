using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderRequireParser(BuilderState builderState) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(require)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from assertionValue in getExpression(state, ExpressionFlags.Standard)
         from scanned2 in state.Scan(@"^(\s*)(else)\b", Color.Whitespace, Color.Keyword)
         from ifFalseValue in getExpression(state, ExpressionFlags.Standard)
         select new BuilderRequire(builderState, assertionValue, ifFalseValue);
      if (_result is (true, var builderRequire))
      {
         state.AddStatement(builderRequire);
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}