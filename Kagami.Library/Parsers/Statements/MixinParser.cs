using System.Collections.Generic;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements;

public class MixinParser : StatementParser
{
   public override string Pattern => $"^ /'mixin' /(|s+|) /({REGEX_CLASS})";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      Module.Global.ForwardReference(className);
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

      state.SkipEndOfLine();
      state.Advance();

      var mixins = new List<Mixin>();
      while (state.More)
      {
         var mixinIncludesParser = new MixinIncludesParser(mixins);
         var _scan = mixinIncludesParser.Scan(state);
         if (_scan)
         {
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

      state.SkipEndOfLine();
      state.Regress();

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
               else if (_scan.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  break;
               }
            }

            Module.Global.RegisterMixin(new Mixin(className));
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