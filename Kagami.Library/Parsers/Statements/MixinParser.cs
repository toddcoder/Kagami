using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class MixinParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(mixin)(\s+)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      Module.Global.Value.ForwardReference(className);
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      List<Mixin> mixins = [];
      while (state.More)
      {
         var mixinIncludesParser = new MixinIncludesParser(mixins);
         var _scan = mixinIncludesParser.Scan(state);
         if (_scan)
         {
         }
         else if (_scan.Exception is (true, var exception2))
         {
            return exception2;
         }
         else
         {
            break;
         }
      }

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         var builder = new ClassBuilder(className, Parameters.Empty, "", [], false, block, mixins);
         var _register = builder.Register();
         if (_register)
         {
            var cls = new Class(builder);
            state.AddStatement(cls);

            var classItemsParser = new ClassItemsParser(builder);
            while (state.More)
            {
               var _scan = classItemsParser.Scan(state);
               if (_scan)
               {
               }
               else if (_scan.Exception is (true, var exception3))
               {
                  return exception3;
               }
               else
               {
                  break;
               }
            }

            Module.Global.Value.RegisterMixin(new Mixin(className));
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