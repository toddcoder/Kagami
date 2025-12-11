using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TypedOperatorParser : SymbolParser
{
   public TypedOperatorParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(typed)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Operator);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
      {
         builder.Add(new SendMessageSymbol("setType(_<TypeConstraint>)", Precedence.PostfixOperator, false, nil, nil,
            new Expression(new PushObjectSymbol(typeConstraint))));
         return unit;
      }
      else if (_possibleTypeConstraint.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fail("Must provide type constraint");
      }
   }
}