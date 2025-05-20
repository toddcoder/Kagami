using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class AssignToNewFieldParser : EndingInExpressionParser
{
   protected bool mutable;
   protected string fieldName = "";
   protected Maybe<TypeConstraint> _typeConstraint = nil;

   public override string Pattern => $"^ /('let' | 'var') /(/s+) /({REGEX_FIELD}) /b";

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      mutable = tokens[1].Text == "var";
      fieldName = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _parsedTypeConstraint = parseTypeConstraint(state);
      if (_parsedTypeConstraint is (true, var parsedTypeConstraint))
      {
         _typeConstraint= parsedTypeConstraint;
      }
      else if (_parsedTypeConstraint.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         _typeConstraint = nil;
      }

      var _scan = state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Structure);
      if (_scan)
      {
         state.CommitTransaction();
         return unit;
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.RollBackTransaction();
         return nil;
      }
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      state.AddStatement(new AssignToNewField(mutable, fieldName, expression, _typeConstraint));
      return unit;
   }
}