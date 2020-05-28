using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class StaticParser : StatementParser
   {
      ClassBuilder classBuilder;

      public StaticParser(ClassBuilder classBuilder) => this.classBuilder = classBuilder;

      public override string Pattern => $"^ /'object' /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);

         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
         {
            var className = classBuilder.UserClass.Name;
            var metaClassName = $"__$meta{className}";
            var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", new Expression[0], false, block,
               new List<Mixin>());
            if (metaClassBuilder.Register().ValueOrOriginal(out _, out var original))
            {
               var classItemsParser = new ClassItemsParser(metaClassBuilder);
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

               var metaClass = new MetaClass(className, metaClassBuilder);
               state.AddStatement(metaClass);

               return Unit.Matched();
            }
            else
            {
               return original;
            }
         }
         else
         {
            return asUnit;
         }
      }
   }
}