using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ClassParser : StatementParser
   {
      public override string Pattern => $"^ /'class' /(|s+|) /({REGEX_CLASS}) /'('?";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var className = tokens[3].Text;
         var hasParameters = tokens[4].Text == "(";
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

         Parameters parameters;

         if (hasParameters)
            if (getParameters(state).If(out parameters, out var parametersOriginal)) { }
            else if (parametersOriginal.IsNotMatched)
               parameters = new Parameters(0);
            else
               return parametersOriginal.ExceptionAs<Unit>();
         else
            parameters = Parameters.Empty;

         var parentClassParser = new ParentClassParser();

         var parentClassName = "";
         var arguments = new Expression[0];
         if (parentClassParser.Scan(state).If(out _, out var isNotMatched, out var exception))
            (parentClassName, _, arguments) = parentClassParser.Parent;
         else if (!isNotMatched)
            return failedMatch<Unit>(exception);

         var traits = new Hash<string, TraitClass>();
         while (state.More)
         {
            var traitImplementsParser = new TraitImplementsParser(traits);
            if (traitImplementsParser.Scan(state).If(out _, out isNotMatched, out exception)) { }
            else if (isNotMatched)
               break;
            else
               return failedMatch<Unit>(exception);
         }

         Module.Global.ForwardReference(className);
         state.SkipEndOfLine();
         if (getBlock(state).If(out var block, out var original))
         {
            var builder = new ClassBuilder(className, parameters, parentClassName, arguments, block);
            if (builder.Register().If(out _, out var registerOriginal))
            {
               var cls = new Class(builder);
               if (testImplementation(builder.UserClass, traits).IfNot(out exception))
                  return failedMatch<Unit>(exception);
               else
                  state.AddStatement(cls);

               var classItemsParser = new ClassItemsParser(builder);
               while (state.More)
                  if (classItemsParser.Scan(state).If(out _, out isNotMatched, out exception)) { }
                  else if (isNotMatched)
                     break;
                  else
                     return failedMatch<Unit>(exception);

               return Unit.Matched();
            }
            else
               return registerOriginal.Unmatched<Unit>();
         }
         else
            return original.Unmatched<Unit>();
      }

      static IResult<Unit> testImplementation(UserClass userClass, Hash<string, TraitClass> traits)
      {
         foreach (var item in traits)
            if (item.Value.RegisterImplementor(userClass).IfNot(out var exception))
               return failure<Unit>(exception);

         return Unit.Success();
      }
   }
}