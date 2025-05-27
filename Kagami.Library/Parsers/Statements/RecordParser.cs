using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class RecordParser : StatementParser
{
   //public override string Pattern => $"^ /'record' /(/s+) /({REGEX_CLASS}) /'('";

   [GeneratedRegex($@"^(record)(\s+)({REGEX_CLASS})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         var parentClassParser = new ParentClassParser();

         var parentClassName = "";
         Expression[] arguments = [];
         var _scan = parentClassParser.Scan(state);
         if (_scan)
         {
            (parentClassName, _, arguments) = parentClassParser.Parent;
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }

         Module.Global.Value.ForwardReference(className);

         var builder = new ClassBuilder(className, parameters, parentClassName, arguments, false, new Block(), new List<Mixin>());
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
      else if (_parameters.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fail("parameters required");
      }
   }
}