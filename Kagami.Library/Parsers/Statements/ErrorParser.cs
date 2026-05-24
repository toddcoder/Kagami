using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ErrorParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(error)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.ClearParameters();

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

      var isFixed = (bool)state.Scan(@"^(\s+)(fixed)", Color.Whitespace, Color.Keyword);

      var _block = getBlock(state, true);
      if (_block is (true, var block))
      {
         var anyMessage = parameters.Any(p => p.Name == "message");
         var anyCallStack = parameters.Any(p => p.Name == "callStack");
         if (!anyMessage)
         {
            block.Insert(new AssignToNewField(false, "message", Expression.FromSymbol(new StringSymbol("")), false, false));
         }

         if (!anyCallStack)
         {
            block.Insert(new AssignToNewField(false, "callStack", Expression.FromSymbol(new CallStackSymbol()), false, false));
         }

         if (state.Parameters.Count > 0)
         {
            parameters = parameters.Merge(state.Parameters);
            state.Parameters.Clear();
         }

         var builder = new ClassBuilder(className, parameters, "", [], false, block, isFixed);

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

         builder.AddProtocol("PError");
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