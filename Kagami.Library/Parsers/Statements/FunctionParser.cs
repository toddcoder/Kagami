using Core.Matching;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class FunctionParser : StatementParser
{
   public override string Pattern => $"^ /('override' /s+)? /('func' | 'op' | 'def') /(/s+) (/({REGEX_CLASS_GETTING}) /'.')?" +
      $" /({REGEX_FUNCTION_NAME}) /'('?";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var overriding = tokens[1].Text.StartsWith("override");
      var isOperator = tokens[2].Text == "op";
      var isMacro = tokens[2].Text == "def";

      var className = tokens[4].Text;
      if (className.IsEmpty() && TraitName.IsNotEmpty())
      {
         className = TraitName;
      }

      var functionName = tokens[6].Text;
      var type = tokens[7].Text;

      if (isOperator && !Module.Global.Value.RegisterOperator(functionName))
      {
         return fail($"Operator {functionName} already registered");
      }

      var needsParameters = type == "(";
      if (needsParameters)
      {
         state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable,
            Color.OpenParenthesis);
         if (functionName.IsMatch("^ /w+ '=' $"))
         {
            functionName = $"__${functionName.Drop(-1)}=";
         }
      }
      else
      {
         state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable);
         functionName = $"__${functionName}";
      }

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _parameters = GetAnyParameters(needsParameters, state);
      if (_parameters is (true, var parameters))
      {
         if (state.CurrentSource.IsMatch("^ /s* '|'"))
         {
            return getMatchFunction(state, functionName, parameters, overriding, className);
         }
         else if (state.CurrentSource.StartsWith("("))
         {
            return getCurriedFunction(state, functionName, parameters, overriding, className).Map(f =>
            {
               if (isMacro)
               {
                  state.RegisterMacro(f);
               }
               else
               {
                  state.AddStatement(f);
               }

               return unit;
            });
         }
         else
         {
            return getAnyBlock(state).Map(block =>
            {
               var yielding = state.RemoveYieldFlag();
               state.RemoveReturnType();
               var function = new Function(functionName, parameters, block, yielding, overriding, className);
               if (isMacro)
               {
                  state.RegisterMacro(function);
               }
               else
               {
                  state.AddStatement(function);
               }

               return unit;
            });
         }
      }
      else
      {
         return _parameters.Exception;
      }
   }

   public string TraitName { get; set; } = "";

   public static Optional<Parameters> GetAnyParameters(bool needsParameters, ParseState state)
   {
      return needsParameters ? getParameters(state) : Parameters.Empty;
   }

   protected static Optional<Function> getCurriedFunction(ParseState state, string functionName, Parameters firstParameters,
      bool overriding, string className)
   {
      var parametersStack = new Stack<Parameters>();
      while (state.More)
      {
         var _parameters =
            from prefix in state.Scan("^ /'('", Color.OpenParenthesis)
            from p in getParameters(state)
            select p;
         if (_parameters is (true, var parameters))
         {
            parametersStack.Push(parameters);
         }
         else if (_parameters.Exception is (true, var exception))
         {
            state.RemoveYieldFlag();
            state.RemoveReturnType();

            return exception;
         }
         else
         {
            break;
         }
      }

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         var yielding = state.RemoveYieldFlag();
         state.RemoveReturnType();
         Maybe<LambdaSymbol> _lambdaSymbol = nil;
         while (parametersStack.Count > 0)
         {
            var parameters = parametersStack.Pop();
            _lambdaSymbol = _lambdaSymbol.Map(l => getLambda(parameters, l)) | (() => new LambdaSymbol(parameters, block));
         }

         if (_lambdaSymbol is (true, var lambdaSymbol))
         {
            return new Function(functionName, firstParameters, new Block(new Return(new Expression(lambdaSymbol), nil)), yielding, overriding,
               className);
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return _block.Exception;
      }
   }

   protected static LambdaSymbol getLambda(Parameters parameters, LambdaSymbol previousLambdaSymbol)
   {
      return new(parameters, new Block(new Return(new Expression(previousLambdaSymbol), nil)));
   }

   protected static Optional<Unit> getMatchFunction(ParseState state, string functionName, Parameters parameters, bool overriding,
      string className)
   {
      List<If> list = [];

      var _advance = state.Advance();
      if (_advance)
      {
         state.CreateReturnType();
         while (state.More)
         {
            var caseParser = new CaseParser(parameters[0].Name);
            state.SkipEndOfLine();
            var _scan = caseParser.Scan(state);
            if (_scan)
            {
               if (caseParser.If is (true, var @if))
               {
                  @if.AddReturnIf();
                  list.Add(@if);
               }
            }
            else if (_scan.Exception is (true, var exception))
            {
               state.Regress();
               return exception;
            }
            else
            {
               break;
            }
         }

         if (list.Count == 0)
         {
            state.Regress();
            state.RemoveReturnType();

            return nil;
         }
         else
         {
            var stack = new Stack<If>();
            foreach (var ifStatement in list)
            {
               stack.Push(ifStatement);
            }

            var previousIf = stack.Pop();
            while (stack.Count > 0)
            {
               var current = stack.Pop();
               current.ElseIf = previousIf.Some();
               previousIf = current;
            }

            state.AddStatement(new MatchFunction(functionName, parameters, previousIf, overriding, className));
            state.Regress();
            state.RemoveReturnType();

            return unit;
         }
      }
      else
      {
         return _advance.Exception;
      }
   }
}