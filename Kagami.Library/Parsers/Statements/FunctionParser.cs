using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using Kagami.Library.Objects;
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
      $@"^(\s*){REGEX_HIDDEN}(override\s+)?(func|(?:infix\(\w+\))|prefix|postfix|match)(\s+)(?:({REGEX_CLASS_GETTING_OR_ALIAS})(\.))?({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var overriding = tokens[3].Text.StartsWith("override");
      var operatorText = tokens[4].Text;
      var isOperator = operatorText.StartsWith("infix") || operatorText is "prefix" or "postfix";
      var isMatch = tokens[4].Text == "match";

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
         if (isMatch)
         {
            var parameterName = "__$0";
            var variadicParameter = new Parameter(false, false, "", parameterName, nil, nil, false, false, false)
            {
               Variadic = true,
               Singleton = true
            };
            var newParameters = new Parameters(variadicParameter);
            return getMatchFunction(state, functionName, newParameters, overriding, className, isFixed, isHidden, SelfAlias);
         }
         else if (state.CurrentSource.StartsWith('('))
         {
            var _curriedFunction = getCurriedFunction(state, functionName, parameters, overriding, className, isHidden, annotations, SelfAlias);
            if (_curriedFunction is (true, var curriedFunction))
            {
               curriedFunction.IsFixed = isFixed;
               _function = curriedFunction;
               state.AddStatement(curriedFunction);

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

               var function = new Function(functionName, parameters, isHidden, block, yielding, overriding, className)
               {
                  IsFixed = isFixed, Annotations = annotations, SelfAlias = SelfAlias
               };
               _function = function;
               state.AddStatement(function);

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
            _lambdaSymbol = _lambdaSymbol.Map(l => getLambda(parameters, l)) | (() => new LambdaSymbol(parameters, block));
         }

         if (_lambdaSymbol is (true, var lambdaSymbol))
         {
            return new Function(functionName, firstParameters, isHidden, new Block(new Return(new Expression(lambdaSymbol), nil)), yielding,
               overriding, className) { Annotations = annotations, SelfAlias = selfAlias };
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
      string className, bool isFixed, bool isHidden, string selfAlias)
   {
      List<If> list = [];

      Maybe<TypeConstraint> _typeConstraint = parseTypeConstraint(state).Map(ptc => ptc.Maybe);

      state.CreateReturnType();
      while (state.More)
      {
         var caseParser = new WhenParser(parameters[0].Name);
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
            return exception;
         }
         else
         {
            break;
         }
      }

      if (list.Count == 0)
      {
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

         previousIf.Else = new Block(new FailedMatch());

         state.AddStatement(new MatchFunction(functionName, parameters, isHidden, previousIf, _typeConstraint, overriding, className)
            { IsFixed = isFixed, SelfAlias = selfAlias });
         state.RemoveReturnType();

         return unit;
      }
   }

   public string SelfAlias { get; set; } = "";
}