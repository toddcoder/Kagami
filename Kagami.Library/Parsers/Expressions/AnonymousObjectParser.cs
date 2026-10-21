using Core.Monads;
using Kagami.Library.Invokables;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AnonymousObjectParser : SymbolParser
{
   public AnonymousObjectParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(new)\b(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.ClearParameters();
      var className = newLabel("class");
      var hasParameters = tokens[3].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      Module.Global.Value.ForwardReference(className);

      Parameters parameters;

      if (hasParameters)
      {
         var _parameters = getParameters(state);
         if (_parameters)
         {
            parameters = _parameters;
         }
         else if (_parameters.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            parameters = [with(0)];
         }
      }
      else
      {
         parameters = Parameters.Empty;
      }

      var parentClassParser = new ParentClassParser();

      var parentClassName = "";
      var initialize = false;
      Expression[] arguments = [];
      var _scan = parentClassParser.Scan(state);
      if (_scan)
      {
         (parentClassName, initialize, arguments) = parentClassParser.Parent;
      }
      else if (_scan.Exception is (true, var exception))
      {
         return exception;
      }

      (string functionName, Expression constructor)[] uses = [];
      var multiDelegateParser = new MultiDelegateParser();
      if (multiDelegateParser.Scan(state))
      {
         uses = multiDelegateParser.Uses;
      }

      var _block = getBlock(state, true);
      if (_block is (true, var block))
      {
         if (state.Parameters.Count > 0)
         {
            parameters = parameters.Merge(state.Parameters);
            state.Parameters.Clear();
         }

         Expression[] callArguments = [];
         if (hasParameters)
         {
            var _open = state.Scan(@"^(\()", Color.OpenParenthesis);
            if (!_open)
            {
               return _open.Exception;
            }

            var _arguments = getArguments(state, ExpressionFlags.Standard);
            if (_arguments)
            {
               callArguments = _arguments;
            }
            else
            {
               return _arguments.Exception;
            }
         }

         var classBuilder = new ClassBuilder(className, parameters, parentClassName, arguments, initialize, block, true);

         var classItemsParser = new ClassItemsParser(classBuilder, true);
         while (state.More)
         {
            var _scan3 = classItemsParser.Scan(state);
            if (_scan3)
            {
            }
            else if (_scan3.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }

         foreach (var (functionName, constructor) in uses)
         {
            classBuilder.RegisterDelegate(functionName, constructor);
         }

         var _register = classBuilder.Register();
         if (_register)
         {
            var cls = new Class(classBuilder);
            state.AddStatement(cls);

            builder.Add(new InvokeSymbol(className, callArguments, nil, false));

            return unit;
         }
         else
         {
            return _register.Exception;
         }
      }
      else
      {
         return _block.Exception;
      }
   }
}