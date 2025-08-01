using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class PendingInvokeParser : SymbolParser
{
   public PendingInvokeParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex($@"^(\s*)(\^)({REGEX_FUNCTION_NAME})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var functionName = tokens[3].Text;
      if (functionName == @"\/")
      {
         return nil;
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Invokable, Color.OpenParenthesis);

         var _argumentsPlusLambda = getArgumentsPlusLambda(state, builder.Flags);
         if (_argumentsPlusLambda is (true, var (arguments, possibleLambda)))
         {
            arguments = [new Expression(new FieldSymbol("__$0")), .. arguments];
            if (state.BlockFollows())
            {
               state.Scan("^(:)", Color.Structure);
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
                     var invokeSymbol = new NewObjectSymbol(tempObjectField, functionName, outerExpression);
                     builder.Add(new PendingInvokeSymbol(invokeSymbol));
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
               var invokeSymbol = new MacroInvokeSymbol(function, arguments);
               builder.Add(new PendingInvokeSymbol(invokeSymbol));
            }
            else
            {
               var invokeSymbol = new InvokeSymbol(functionName, arguments, possibleLambda, builder.Flags[ExpressionFlags.Comparisand]);
               builder.Add(new PendingInvokeSymbol(invokeSymbol));
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