using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ExtensionParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(static\s+)?(extension)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isStatic = tokens[2].Text.StartsWith("static");
      var parameterName = tokens[5].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Identifier);

      var _possibleTypeConstraint = parseTypeConstraint(state);
      if (_possibleTypeConstraint is (true, PossibleTypeConstraint.Some { Maybe: (true, var typeConstraint) }))
      {
         var className = typeConstraint.Comparisands[0].Name;
         if (isStatic)
         {
            className = metaName(className);
         }

         if (state.BeginBlock())
         {
            while (state.More)
            {
               var functionParser = new FunctionAndPropertyParsers(className, parameterName);
               var constructorParser = new ExtensionConstructorParser(className);
               var _scanned = functionParser.Scan(state);
               if (_scanned)
               {
                  continue;
               }
               else if (_scanned.Exception is (true, var exception))
               {
                  return exception;
               }

               _scanned = constructorParser.Scan(state);
               if (_scanned)
               {
               }
               else if (_scanned.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  break;
               }
            }

            var _endBlock = state.EndBlock();
            if (_endBlock)
            {
               return unit;
            }
            else if (_endBlock.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               return fail("Only functions or init are allowed");
            }
         }
         else
         {
            return fail("Block expected");
         }
      }
      else
      {
         return fail("Extension type constraint not provided");
      }
   }
}