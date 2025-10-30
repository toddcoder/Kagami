using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class EmptyValueParser : SymbolParser
{
   public EmptyValueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\{}|\{:}|\(\)|\[\]|\[::\])")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var source = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Collection);

      var _parsedTypeConstraint = parseTypeConstraint(state);

      if (_parsedTypeConstraint is (true, var possibleTypeConstraint))
      {
         switch (source)
         {
            case "[]":
               builder.Add(new EmptyArraySymbol(possibleTypeConstraint.Maybe));
               break;
            case "{:}":
               builder.Add(new EmptyDictionarySymbol(possibleTypeConstraint.Maybe));
               break;
            case "()":
               builder.Add(new EmptyTupleSymbol());
               break;
            case "[::]":
               builder.Add(new EmptyListSymbol());
               break;
            case "{}":
               builder.Add(new EmptySetSymbol(possibleTypeConstraint.Maybe));
               break;
            default:
               return nil;
         }

         return unit;
      }
      else
      {
         return _parsedTypeConstraint.Exception;
      }
   }
}