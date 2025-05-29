using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ExpressionStatementParser : StatementParser
{
   protected bool returnExpression;
   protected Maybe<TypeConstraint> _typeConstraint;

   public ExpressionStatementParser(bool returnExpression, Maybe<TypeConstraint> _typeConstraint)
   {
      this.returnExpression = returnExpression;
      this._typeConstraint = _typeConstraint;
   }

   [GeneratedRegex(".+")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var flags = ExpressionFlags.Standard;
      if (returnExpression)
      {
         flags |= ExpressionFlags.OmitSendMessageAssign;
      }

      var _expression = getExpression(state, flags);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new ExpressionStatement(expression, returnExpression, _typeConstraint));
         state.Scan(@"^(\s*)(\r\n|\r|\n|;)", Color.Whitespace, Color.Structure);

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }

   public override bool UpdateIndexOnParseOnly => true;
}