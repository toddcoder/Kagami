using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class RecordParser : StatementParser
   {
      public override string Pattern => $"^ /'record' /(|s+|) /({REGEX_CLASS}) /'('";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var className = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

         if (getParameters(state).If(out var parameters, out var parametersOriginal))
         {
            var parentClassParser = new ParentClassParser();

            var parentClassName = "";
            var arguments = new Expression[0];
            if (parentClassParser.Scan(state).If(out _, out var isNotMatched, out var exception))
               (parentClassName, _, arguments) = parentClassParser.Parent;
            else if (!isNotMatched)
               return failedMatch<Unit>(exception);

            Module.Global.ForwardReference(className);

            var builder = new ClassBuilder(className, parameters, parentClassName, arguments, new Block());
            if (builder.Register().If(out _, out var registerOriginal))
            {
               var cls = new Class(builder);
               state.AddStatement(cls);

               return Unit.Matched();
            }
            else
               return registerOriginal;
         }
         else if (parametersOriginal.IsNotMatched)
            return "parameters required".FailedMatch<Unit>();
         else
            return parametersOriginal.ExceptionAs<Unit>();
      }
   }
}