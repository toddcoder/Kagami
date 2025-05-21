using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class InvokeParser : SymbolParser
{
   public InvokeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(|s|) /({REGEX_FUNCTION_NAME}) /'('";

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
            if (state.BlockFollows())
            {
               state.Scan("^ /':'", Color.Structure);
               var _result = state.BeginBlock();
               if (_result)
               {
                  var tempObjectField = newLabel("object");
                  var outerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
                  var setPropertyParser = new SetPropertyParser(builder, tempObjectField, outerBuilder);
                  while (state.More)
                  {
                     var _property = setPropertyParser.Scan(state);
                     if (_property)
                     {
                     }
                     else if (_property.Exception is (true, var exception))
                     {
                        return exception;
                     }
                     else
                     {
                        break;
                     }
                  }

                  _result = state.EndBlock();
                  if (!_result)
                  {
                     return _result.Exception;
                  }

                  var _outerExpression = outerBuilder.ToExpression();
                  if (_outerExpression is (true, var outerExpression))
                  {
                     builder.Add(new NewObjectSymbol(tempObjectField, functionName, outerExpression));
                  }
                  else
                  {
                     return _outerExpression.Exception;
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