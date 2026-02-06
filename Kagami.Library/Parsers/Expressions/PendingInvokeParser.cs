using Core.Matching;
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

   [GeneratedRegex($@"^(\s*)(%)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var functionName = tokens[3].Text;
      var hasParameter = tokens[4].Text == "(";
      if (functionName == @"\/")
      {
         return nil;
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Invokable, Color.OpenParenthesis);

         var _argumentsPlusLambda = getArgumentsPlusLambda(state, builder.Flags);
         var (arguments, possibleLambda) = _argumentsPlusLambda.DefaultTo(_ => ([], nil));
         if (hasParameter)
         {
            arguments = [new Expression(new FieldSymbol("__$0")), .. arguments];
            var invokeSymbol = new InvokeSymbol(functionName, arguments, possibleLambda, builder.Flags[ExpressionFlags.Comparisand]);
            builder.Add(new PendingInvokeSymbol(invokeSymbol, hasParameter));
            return unit;
         }

         if (functionName.IsMatch("^ ['A-Z']") && state.BlockFollows())
         {
            var _result = state.BeginBlock();
            if (_result)
            {
               var tempObjectField = newLabel("object");
               var _taggedExpressions = getTaggedExpressions(state, REGEX_BLOCK_END);
               if (_taggedExpressions is (true, var taggedExpressions))
               {
                  builder.Add(new NewObjectSymbol(tempObjectField, functionName, arguments, taggedExpressions));
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

         return unit;
      }
   }
}