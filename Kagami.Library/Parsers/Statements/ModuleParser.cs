using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements;

public class ModuleParser : StatementParser
{
   public override string Pattern => $"^ /'module' /(|s+|) /({REGEX_CLASS}) /({REGEX_EOL})";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace);

      var parameters = Parameters.Empty;

      var parentClassName = "";
      Expression[] arguments = [];
      Module.Global.ForwardReference(className);

      var builder = new ClassBuilder(className, parameters, parentClassName, arguments, false, new Block(), new List<Mixin>());
      var _register = builder.Register();
      if (_register)
      {
         var cls = new Class(builder);
         state.AddStatement(cls);

         var _block = getBlock(state);
         if (_block is (true, var block))
         {
            var metaClassName = $"__$meta{className}";
            var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", [], false, block, new List<Mixin>());
            _register = metaClassBuilder.Register();
            if (_register)
            {
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
      else
      {
         return _register.Exception;
      }
   }
}