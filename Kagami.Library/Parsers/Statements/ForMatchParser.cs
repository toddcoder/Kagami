using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using System.Text.RegularExpressions;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ForMatchParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(for\s+match)(?:(\s+)({REGEX_FIELD})(\s+in))?\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var matchField = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Keyword);
      var _result =
         from sourceValue in getExpression(state, ExpressionFlags.Standard | ExpressionFlags.OmitIf | ExpressionFlags.OmitNot)
         select sourceValue;
      if (_result is (true, var source))
      {
         var _scan = state.BeginBlock();
         if (!_scan)
         {
            return state.SetException(messageNoBeginBlock("for match"), _scan.Exception);
         }

         var forField = newLabel("for");
         if (matchField.IsEmpty())
         {
            matchField = newLabel("match");
         }

         var comparisand = new Expression(new PlaceholderSymbol("-" + forField));
         var assignment = new Expression(new FieldSymbol(forField));

         state.AddStatement(new PushFrameStatement());

         state.PushStatements();
         var whenParser = new WhenParser("", false, false, matchField, false, CaseType.Statement);
         _scan = whenParser.Scan(state);
         if (_scan)
         {
            addMatchElse(whenParser.If);
            var _endBlock = state.EndBlock();
            if (_endBlock && whenParser.If is (true, var ifStatement))
            {
               state.PopStatements();
               var block = new Block([new AssignToNewField(true, matchField, assignment, false, false), ifStatement]);
               state.AddStatement(new For(comparisand, source, block, new PossibleIfExpression.None(), nil, nil));

               state.AddStatement(new PopFrameStatement());

               return unit;
            }
            else if (_endBlock.Exception is (true, var exception))
            {
               return state.SetException(messageNoBeginBlock("for match"), exception);
            }
         }
         else
         {
            return _scan.Exception;
         }
      }
      else
      {
         return _result.Exception;
      }

      return nil;
   }
}