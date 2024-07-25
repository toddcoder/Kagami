using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;

namespace Kagami.Library.Parsers.Statements
{
   public class ClassParser : StatementParser
   {
      public override string Pattern => $"^ /'class' /(|s+|) /({REGEX_CLASS}) /'('?";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var className = tokens[3].Text;
         var hasParameters = tokens[4].Text == "(";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

         Parameters parameters;

         if (hasParameters)
         {
            if (getParameters(state).ValueOrCast<Unit>(out parameters, out var asUnit)) { }
            else if (asUnit.IsNotMatched)
            {
               parameters = new Parameters(0);
            }
            else
            {
               return asUnit;
            }
         }
         else
         {
            parameters = Parameters.Empty;
         }

         state.SkipEndOfLine();

         state.Advance();
         var parentClassParser = new ParentClassParser();

         var parentClassName = "";
         var initialize = false;
         var arguments = new Expression[0];
         if (parentClassParser.Scan(state).If(out _, out var anyException))
         {
            (parentClassName, initialize, arguments) = parentClassParser.Parent;
         }
         else if (anyException.If(out var exception))
         {
            state.Regress();
            return failedMatch<Unit>(exception);
         }

         var mixins = new List<Mixin>();
         while (state.More)
         {
            var mixinIncludesParser = new MixinIncludesParser(mixins);
            if (mixinIncludesParser.Scan(state).If(out _, out anyException)) { }
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

         Module.Global.ForwardReference(className);

         state.SkipEndOfLine();
         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit2))
         {
            var builder = new ClassBuilder(className, parameters, parentClassName, arguments, initialize, block, mixins);
            if (builder.Register().ValueOrOriginal(out _, out asUnit2))
            {
               var cls = new Class(builder);
               state.AddStatement(cls);

               var classItemsParser = new ClassItemsParser(builder);
               while (state.More)
               {
                  if (classItemsParser.Scan(state).If(out _, out anyException)) { }
                  else if (anyException.If(out var exception))
                  {
                     return failedMatch<Unit>(exception);
                  }
                  else
                  {
                     break;
                  }
               }

               return Unit.Matched();
            }
            else
            {
               return asUnit2;
            }
         }
         else
         {
            return asUnit2;
         }
      }
   }
}