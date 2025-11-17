using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
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

      if (isSet)
      {
         if (state.Scan("^(:})", Color.Collection))
         {
            var _parsedTypeConstraint = parseTypeConstraint(state);
            if (_parsedTypeConstraint is (true, var possibleTypeConstraint))
            {
               builder.Add(new EmptyDictionarySymbol(possibleTypeConstraint.Maybe));
               state.CommitTransaction();
               return unit;
            }
         }

         if (state.Scan("^(})", Color.Collection))
         {
            var _parsedTypeConstraint = parseTypeConstraint(state);
            if (_parsedTypeConstraint is (true, var possibleTypeConstraint))
            {
               builder.Add(new EmptySetSymbol(possibleTypeConstraint.Maybe));
               state.CommitTransaction();
               return unit;
            }
         }

         var _leftParsedTypeConstraint = parseTypeConstraint(state);
         if (_leftParsedTypeConstraint is (true, { Maybe: (true, var leftTypeConstraint) }))
         {
            if (state.Scan("^(:)", Color.Collection))
            {
               var _rightParsedTypeConstraint = parseTypeConstraint(state);
               if (_rightParsedTypeConstraint is (true, { Maybe: (true, var rightTypeConstraint) }))
               {
                  if (state.Scan("^(})", Color.Collection))
                  {
                     var _lambda = getAnyLambda(state, builder.Flags);
                     if (_lambda is (true, var lambda))
                     {
                        builder.Add(new EmptyMemoSymbol(lambda, leftTypeConstraint.Append(rightTypeConstraint)));
                     }
                     else
                     {
                        builder.Add(new EmptyDictionarySymbol(leftTypeConstraint.Append(rightTypeConstraint)));
                     }

                     state.CommitTransaction();
                     return unit;
                  }
                  else
                  {
                     state.RollBackTransaction();
                     return nil;
                  }
               }
               else if (_rightParsedTypeConstraint.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  state.RollBackTransaction();
                  return nil;
               }
            }
            else if (state.Scan("^(})", Color.Collection))
            {
               builder.Add(new EmptySetSymbol(leftTypeConstraint));
               state.CommitTransaction();
               return unit;
            }
         }
         else if (_leftParsedTypeConstraint.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            state.RollBackTransaction();
            return nil;
         }
      }
      else if (isArray)
      {
         var _possibleTypeConstraint = parseTypeConstraint(state);
         if (_possibleTypeConstraint is (true, { Maybe: (true, var typeConstraint) }))
         {
            if (state.Scan("^(])", Color.Collection))
            {
               builder.Add(new EmptyArraySymbol(typeConstraint));
               state.CommitTransaction();
               return unit;
            }
            else if (_possibleTypeConstraint.Exception is (true, var exception))
            {
               return exception;
            }
            else if (state.Scan("^(])", Color.Collection) && parseTypeConstraint(state) is (true, var possibleTypeConstraint))
            {
               builder.Add(new EmptyArraySymbol(possibleTypeConstraint.Maybe));
               state.CommitTransaction();
               return unit;
            }
            else
            {
               state.RollBackTransaction();
               return nil;
            }
         }
         else if (_possibleTypeConstraint.Exception is (true, var exception))
         {
            return exception;
         }
         else if (state.Scan("^(])", Color.Collection) && parseTypeConstraint(state) is (true, var possibleTypeConstraint))
         {
            builder.Add(new EmptyArraySymbol(possibleTypeConstraint.Maybe));
            state.CommitTransaction();
            return unit;
         }
         else
         {
            state.RollBackTransaction();
            return nil;
         }
      }

      state.RollBackTransaction();
      return nil;
   }
}