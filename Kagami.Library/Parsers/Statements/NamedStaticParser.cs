using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class NamedStaticParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(object|mixin)(\s+)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isMixin = tokens[2].Text == "mixin";
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      Module.Global.Value.ForwardReference(className);

      var builder = new ClassBuilder(className, Parameters.Empty, "", [], false, new Block());
      var _register = builder.Register();
      if (_register)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         var _block = getBlock(state);
         if (_block is (true, var block))
         {
            var metaClassName = $"__$meta{className}";
            var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", [], false, block);
            _register = metaClassBuilder.Register();
            if (_register)
            {
               var classItemsParser = new ClassItemsParser(metaClassBuilder, !isMixin);
               while (state.More)
               {
                  var _scan = classItemsParser.Scan(state);
                  if (_scan)
                  {
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

               var metaClass = new MetaClass(className, metaClassBuilder);
               if (isMixin)
               {
                  Module.RegisterMixin(className, metaClass);
               }

               state.AddStatement(metaClass);

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

      return unit;
   }
}