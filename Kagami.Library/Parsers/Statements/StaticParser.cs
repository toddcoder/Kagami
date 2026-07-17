using System.Text.RegularExpressions;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class StaticParser : StatementParser
{
   protected ClassBuilder classBuilder;

   public StaticParser(ClassBuilder classBuilder)
   {
      this.classBuilder = classBuilder;
   }

   [GeneratedRegex(@"^(\s*)(static)\b(?!\s+extension\b)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         var className = classBuilder.ClassName;
         var metaClassName = metaName(className);
         var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", [], false, block);
         var _register = metaClassBuilder.Register();
         if (_register)
         {
            var classItemsParser = new ClassItemsParser(metaClassBuilder, true);
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
}