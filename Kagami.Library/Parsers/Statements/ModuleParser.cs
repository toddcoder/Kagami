﻿using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements
{
   public class ModuleParser : StatementParser
   {
      public override string Pattern => $"^ /'module' /(|s+|) /({REGEX_CLASS}) /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var className = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace);

         var parameters = Parameters.Empty;

         var parentClassName = "";
         var arguments = new Expression[0];
         Module.Global.ForwardReference(className);

         var builder = new ClassBuilder(className, parameters, parentClassName, arguments, false, new Block(), new List<Mixin>());
         if (builder.Register().ValueOrOriginal(out _, out var registerOriginal))
         {
            var cls = new Class(builder);
            state.AddStatement(cls);

            if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
            {
               var metaClassName = $"__$meta{className}";
               var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", new Expression[0], false, block,
                  new List<Mixin>());
               if (metaClassBuilder.Register().ValueOrOriginal(out _, out registerOriginal))
               {
                  var metaClass = new MetaClass(className, metaClassBuilder);
                  state.AddStatement(metaClass);

                  return Unit.Matched();
               }
               else
               {
                  return registerOriginal;
               }
            }
            else
            {
               return asUnit;
            }
         }
         else
         {
            return registerOriginal;
         }
      }
   }
}