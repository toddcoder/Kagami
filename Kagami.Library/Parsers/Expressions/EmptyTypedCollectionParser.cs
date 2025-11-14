using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class EmptyTypedCollectionParser : SymbolParser
{
   public EmptyTypedCollectionParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)([\[\{{])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var isSet = tokens[2].Text == "{";
      var isArray = tokens[2].Text == "[";
      state.BeginTransaction();
      state.Colorize(tokens, Color.Whitespace, Color.Collection);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
      {
         if (isSet && state.Scan("^(:)", Color.Collection))
         {
            IObject collection;
            var _possibleTypeConstraint2 = parseTypeConstraint(state);
            if (_possibleTypeConstraint2 is (true, { Maybe: (true, var typeConstraint2) }))
            {
               var dictionary = Dictionary.Empty;
               dictionary.TypeConstraint = typeConstraint.Append(typeConstraint2);
               collection = dictionary;
            }
            else
            {
               var set = Set.Empty;
               set.TypeConstraint = typeConstraint;
               collection = set;
            }

            state.Scan("^(})", Color.Collection);
            state.CommitTransaction();
            builder.Add(new PushObjectSymbol(collection));
         }
         else if (isArray)
         {
            var array = KArray.Empty;
            array.TypeConstraint = typeConstraint;
            builder.Add(new PushObjectSymbol(array));
            state.Scan(@"^(\])", Color.Collection);
            state.CommitTransaction();
         }
      }
      else if (_possibleTypeConstraint.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.RollBackTransaction();
      }

      return unit;
   }
}