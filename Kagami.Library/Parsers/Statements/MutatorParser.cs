using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class MutatorParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(\s*)(\.=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);
      state.StartingValueSymbol = new FieldSymbol(fieldName);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new AssignToField(fieldName, nil, expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}