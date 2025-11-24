using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InvokeParser : SymbolParser
{
   public InvokeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)({REGEX_FUNCTION_NAME})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var functionName = tokens[2].Text;
      if (functionName == @"\/")
      {
         return nil;
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Invokable, Color.OpenParenthesis);

         var _argumentsPlusLambda = getArgumentsPlusLambda(state, builder.Flags);
         if (_argumentsPlusLambda is (true, var (arguments, possibleLambda)))
         {
            if (!builder.Flags[ExpressionFlags.Comparisand] && functionName.IsMatch("^ ['A-Z']") && state.BlockFollows())
            {
               var _result = state.BeginBlock(Color.OpenParenthesis);
               if (_result)
               {
                  var tempObjectField = newLabel("object");
                  var _taggedExpressions = getTaggedExpressions(state, REGEX_BLOCK_END);
                  if (_taggedExpressions is (true, var taggedExpressions))
                  {
                     builder.Add(new NewObjectSymbol(tempObjectField, functionName, taggedExpressions));
                  }
                  else
                  {
                     return _taggedExpressions.Exception;
                  }
               }
               else
               {
                  return _result.Exception;
               }
            }
            else if (state.Macro(functionName) is (true, var function))
            {
               builder.Add(new MacroInvokeSymbol(function, arguments));
            }
            else
            {
               builder.Add(new InvokeSymbol(functionName, arguments, possibleLambda, builder.Flags[ExpressionFlags.Comparisand]));
            }

            return unit;
         }
         else
         {
            return _argumentsPlusLambda.Exception;
         }
      }
   }
}