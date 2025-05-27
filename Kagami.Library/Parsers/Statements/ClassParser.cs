using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ClassParser : StatementParser
{
   //public override string Pattern => $"^ /'class' /(/s+) /({REGEX_CLASS}) /'('?";

   [GeneratedRegex($@"^(class)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      var hasParameters = tokens[4].Text == "(";
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

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

      List<Mixin> mixins = [];
      while (state.More)
      {
         var mixinIncludesParser = new MixinIncludesParser(mixins);
         var _scan2 = mixinIncludesParser.Scan(state);
         if (_scan2)
         {
         }
         else if (_scan2.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      Module.Global.Value.ForwardReference(className);

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         var builder = new ClassBuilder(className, parameters, parentClassName, arguments, initialize, block, mixins);
         var _register = builder.Register();
         if (_register)
         {
            var cls = new Class(builder);
            state.AddStatement(cls);

            var classItemsParser = new ClassItemsParser(builder);
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