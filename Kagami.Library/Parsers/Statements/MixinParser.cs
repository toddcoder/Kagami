using System.Collections.Generic;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements
{
   public class MixinParser : StatementParser
   {
      public override string Pattern => $"^ /'mixin' /(|s+|) /({REGEX_CLASS})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
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
            if (mixinIncludesParser.Scan(state).If(out _, out var anyException)) { }
            else if (anyException.If(out var exception))
            {
               state.Regress();
               return failedMatch<Unit>(exception);
            }
            else
            {
               break;
            }
         }

         state.SkipEndOfLine();
         state.Regress();

         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
         {
            var builder = new ClassBuilder(className, Parameters.Empty, "", new Expression[0], false, block, mixins);
            if (builder.Register().ValueOrOriginal(out _, out asUnit))
            {
               var cls = new Class(builder);
               state.AddStatement(cls);

               var classItemsParser = new ClassItemsParser(builder);
               while (state.More)
               {
                  if (classItemsParser.Scan(state).If(out _, out var anyException)) { }
                  else if (anyException.If(out var exception))
                  {
                     return failedMatch<Unit>(exception);
                  }
                  else
                  {
                     break;
                  }
               }

               Module.Global.RegisterMixin(new Mixin(className));
               return Unit.Matched();
            }
            else
            {
               return asUnit;
            }
         }
         else
         {
            return asUnit;
         }
      }
   }
}