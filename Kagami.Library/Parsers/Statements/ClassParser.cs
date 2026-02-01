using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ClassParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(class|annotation)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.ClearParameters();

      var isAnnotation = tokens[2].Text == "annotation";
      if (isAnnotation)
      {
         state.AllowReorder = false;
      }

      var className = tokens[4].Text;
      var hasParameters = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

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
            parameters = new Parameters(0);
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

      var isFixed = (bool)state.Scan(@"^(\s+)(fixed)", Color.Whitespace, Color.Keyword);

      var _block = getBlock(state, true);
      if (_block is (true, var block))
      {
         if (state.Parameters.Count > 0)
         {
            parameters = parameters.Merge(state.Parameters);
            state.Parameters.Clear();
         }

         var builder = new ClassBuilder(className, parameters, parentClassName, arguments, initialize, block, isFixed || isAnnotation);

         var classItemsParser = new ClassItemsParser(builder, true);
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

         var _register = builder.Register();
         if (_register)
         {
            var cls = new Class(builder);
            state.AddStatement(cls);

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