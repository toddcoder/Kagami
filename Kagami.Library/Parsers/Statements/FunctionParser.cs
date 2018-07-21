using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class FunctionParser : StatementParser
   {
      public override string Pattern => $"^ /('override' /s+)? /('func' | 'op' | 'def') /(/s+) (/({REGEX_CLASS_GETTING}) /'.')?" +
         $" /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var overriding = tokens[1].Text.StartsWith("override");
         var isOperator = tokens[2].Text == "op";
         var isMacro = tokens[2].Text == "def";

         var trait = false;
         var className = tokens[4].Text;
         if (className.IsEmpty() && TraitName.IsNotEmpty())
         {
            className = TraitName;
            trait = true;
         }

         var functionName = tokens[6].Text;
         var type = tokens[7].Text;

         if (isOperator && !Module.Global.RegisterOperator(functionName))
            return $"Operator {functionName} already registered".FailedMatch<Unit>();

         state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable,
            Color.Structure);

         var needsParameters = type == "(";
         if (needsParameters)
         {
            if (functionName.IsMatch("^ /w+ '=' $"))
               functionName = functionName.Skip(-1).set();
         }
         else
            functionName = functionName.get();

         state.CreateYieldFlag();

         if (GetAnyParameters(needsParameters, state).If(out var parameters, out var original))
         {
            if (state.CurrentSource.StartsWith("("))
               return getCurriedFunction(state, functionName, parameters, overriding, className, trait).Map(f =>
               {
                  if (isMacro)
                     state.RegisterMacro(f);
                  else
                     state.AddStatement(f);

                  return Unit.Matched();
               });
            else
               return getAnyBlock(state).Map(block =>
               {
                  var yielding = state.RemoveYieldFlag();
                  var function = new Function(functionName, parameters, block, yielding, overriding, className) { Trait = trait };
                  if (isMacro)
                     state.RegisterMacro(function);
                  else
                     state.AddStatement(function);

                  return Unit.Matched();
               });
         }
         else
            return original.Unmatched<Unit>();
      }

      public string TraitName { get; set; } = "";

      public static IMatched<Parameters> GetAnyParameters(bool needsParameters, ParseState state)
      {
         if (needsParameters)
            return getParameters(state);
         else
            return Parameters.Empty.Matched();
      }

      protected static IMatched<Function> getCurriedFunction(ParseState state, string functionName, Parameters firstParameters,
         bool overriding, string className, bool trait)
      {
         var parametersStack = new Stack<Parameters>();
         while (state.More)
         {
            var result =
               from prefix in state.Scan("^ /'('", Color.Structure)
               from p in getParameters(state)
               select p;
            if (result.If(out var parameters, out var isNotMatched, out var exception))
               parametersStack.Push(parameters);
            else if (isNotMatched)
               break;
            else
            {
               state.RemoveYieldFlag();
               return failedMatch<Function>(exception);
            }
         }

         if (getAnyBlock(state).If(out var block, out var original))
         {
            var yielding = state.RemoveYieldFlag();
            var lambdaSymbol = none<LambdaSymbol>();
            while (parametersStack.Count > 0)
            {
               var parameters = parametersStack.Pop();
               lambdaSymbol = lambdaSymbol.FlatMap(l => getLambda(parameters, l), () => new LambdaSymbol(parameters, block)).Some();
            }

            if (lambdaSymbol.If(out var ls))
               return new Function(functionName, firstParameters, new Block(new Return(new Expression(ls))), yielding,
                  overriding, className) { Trait = trait }.Matched();
            else
               return notMatched<Function>();
         }
         else
            return original.Unmatched<Function>();
      }

      protected static LambdaSymbol getLambda(Parameters parameters, LambdaSymbol previousLambdaSymbol)
      {
         return new LambdaSymbol(parameters, new Block(new Return(new Expression(previousLambdaSymbol))));
      }
   }
}