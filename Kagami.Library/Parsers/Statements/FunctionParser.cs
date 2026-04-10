using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class FunctionParser : StatementParser
{
   protected Maybe<Function> _function = nil;
   protected List<InvokeSymbol> annotations = [];

   [GeneratedRegex(
      $@"^(\s*){REGEX_HIDDEN}(override\s+)?(fn|(?:infix\(\w+\))|prefix|postfix|macro)(\s+)(?:({REGEX_CLASS_GETTING_OR_ALIAS})(\.))?({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var overriding = tokens[3].Text.StartsWith("override");
      var operatorText = tokens[4].Text;
      var isOperator = operatorText.StartsWith("infix") || operatorText is "prefix" or "postfix";
      var isMacro = tokens[4].Text == "macro";

      var className = tokens[6].Text;
      (className, var color) = getClassNameWithColor(className);
      className = ClassName | className;

      var functionName = tokens[8].Text;
      var type = tokens[9].Text;

      annotations = [.. state.Annotations];

      if (isOperator)
      {
         var precedence = operatorText switch
         {
            "prefix" => Precedence.PrefixOperator,
            "postfix" => Precedence.PostfixOperator,
            _ => operatorText.Drop(6).Drop(-1).Trim() switch
            {
               "raise" => Precedence.Raise,
               "multiply" or "divide" => Precedence.MultiplyDivide,
               "range" => Precedence.Range,
               "add" or "subtract" => Precedence.AddSubtract,
               "shift" => Precedence.Shift,
               "boolean" => Precedence.Boolean,
               "and" => Precedence.And,
               "or" => Precedence.Or,
               "format" => Precedence.Format,
               _ => Precedence.ChainedOperator
            }
         };

         if (operatorText.StartsWith("infix"))
         {
            operatorText = operatorText.Keep(5);
         }

         Maybe<OperatorType> _operatorType = operatorText switch
         {
            "infix" => new OperatorType.Infix(functionName, precedence),
            "prefix" => new OperatorType.Prefix(functionName),
            "postfix" => new OperatorType.Postfix(functionName),
            _ => nil
         };
         if (_operatorType is (true, var operatorType))
         {
            if (Module.Global.Value.RegisterOperator(operatorType))
            {
            }
            else
            {
               return operatorAlreadyExists(functionName);
            }
         }
      }

      var needsParameters = type == "(";
      if (needsParameters)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, color, Color.Structure,
            Color.Invokable,
            Color.OpenParenthesis);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Keyword, Color.Whitespace, color, Color.Structure,
            Color.Invokable);
         functionName = $"__${functionName}";
      }

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _parameters = GetAnyParameters(needsParameters, state);
      if (_parameters is (true, var parameters))
      {
         var isFixed = state.Scan(@"^(\s+)(fixed)\b", Color.Whitespace, Color.Keyword);
         if (state.CurrentSource.StartsWith('('))
         {
            var _curriedFunction = getCurriedFunction(state, functionName, parameters, overriding, className, isHidden, annotations, SelfAlias);
            if (_curriedFunction is (true, var curriedFunction))
            {
               curriedFunction.IsFixed = isFixed;
               _function = curriedFunction;
               if (isMacro)
               {
                  state.RegisterMacro(curriedFunction);
               }
               else
               {
                  state.AddStatement(curriedFunction);
               }

               return unit;
            }
            else
            {
               return _curriedFunction.Exception;
            }
         }
         else
         {
            var _block = getAnyBlock(state);
            if (_block is (true, var block))
            {
               var yielding = state.RemoveYieldFlag();
               state.RemoveReturnType();
               if (!yielding)
               {
                  block.AddReturnUnitIf();
               }

               if (SelfAlias.IsNotEmpty())
               {
                  parameters.Append(getSelfParameter(state, SelfAlias));
               }

               var function = new Function(functionName, parameters, isHidden, block, yielding, overriding, className)
               {
                  IsFixed = isFixed, Annotations = annotations
               };
               _function = function;
               if (isMacro)
               {
                  state.RegisterMacro(function);
               }
               else
               {
                  state.AddStatement(function);
               }

               return unit;
            }
            else
            {
               return _block.Exception;
            }
         }
      }
      else
      {
         return _parameters.Exception;
      }
   }

   public Maybe<string> ClassName { get; set; } = nil;

   public static Optional<Parameters> GetAnyParameters(bool needsParameters, ParseState state)
   {
      return needsParameters ? getParameters(state) : Parameters.Empty;
   }

   protected static Optional<Function> getCurriedFunction(ParseState state, string functionName, Parameters firstParameters,
      bool overriding, string className, bool isHidden, List<InvokeSymbol> annotations, string selfAlias)
   {
      var parametersStack = new Stack<Parameters>();
      while (state.More)
      {
         var _parameters =
            from prefix in state.Scan(@"^(\()", Color.OpenParenthesis)
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
            if (selfAlias.IsNotEmpty())
            {
               parameters.Append(getSelfParameter(state, selfAlias));
            }

            _lambdaSymbol = _lambdaSymbol.Map(l => getLambda(parameters, l)) | (() => new LambdaSymbol(parameters, block));
         }

         if (_lambdaSymbol is (true, var lambdaSymbol))
         {
            return new Function(functionName, firstParameters, isHidden, new Block(new Return(new Expression(lambdaSymbol), nil)), yielding,
               overriding, className) { Annotations = annotations };
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

   public string SelfAlias { get; set; } = "";
}