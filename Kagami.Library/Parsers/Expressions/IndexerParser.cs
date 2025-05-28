using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class IndexerParser : SymbolParser
{
   public IndexerParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /'[' /'+'?";

   [GeneratedRegex(@"^(\[)(\+)?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var insert = tokens[2].Text == "+";
      state.Colorize(tokens, Color.OpenParenthesis, Color.Structure);

      var _expressions = getArguments(state, builder.Flags);
      if (_expressions is (true, var expressions))
      {
         var _scan = state.Scan(@$"^(\s*)({REGEX_ASSIGN_OPS})?(=)(?!=)", Color.Whitespace, Color.Operator, Color.Structure);
         if (_scan is (true, var opSource))
         {
            var _expression = getExpression(state, builder.Flags);
            if (_expression is (true, var expression))
            {
               opSource = opSource.DropWhile(" ").Keep(1);
               var operation = matchOperator(opSource) | nil;
               if (!operation && insert)
               {
                  var list = expressions.ToList();
                  list.Add(expression);
                  builder.Add(new SendMessageSymbol("insert(at:_<Int>,value:_)", nil, nil, [.. list]));
               }
               else
               {
                  builder.Add(new IndexSetterSymbol(expressions, expression, operation.Maybe()));
               }

               return unit;
            }
            else
            {
               return _expression.Exception;
            }
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }
         else if (expressions.Length > 0)
         {
            builder.Add(new IndexerSymbol(expressions));
         }
         else
         {
            return nil;
         }

         return unit;
      }
      else
      {
         return _expressions.Exception;
      }
   }
}