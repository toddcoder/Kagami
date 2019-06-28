using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ClassReferenceParser : SymbolParser
   {
      public override string Pattern => $"^ /(|s|) /({REGEX_CLASS_GETTING}) /b";

      public ClassReferenceParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var className = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Class);

         if (Module.Global.Class(className).If(out var cls))
         {
            builder.Add(new ClassSymbol(cls.Name));
            return Unit.Matched();
         }
         else if (Module.Global.FullDataComparisandName(className).If(out var _))
         {
            builder.Add(new FieldSymbol(className));
            return Unit.Matched();
         }
         else if (Module.Global.Forwarded(className))
         {
            builder.Add(new ClassSymbol(className));
            return Unit.Matched();
         }
			else if (state.ContainsPattern(className))
         {
	         builder.Add(new FieldSymbol(className));
	         return Unit.Matched();
         }
         else
         {
	         return notMatched<Unit>();
         }
      }
   }
}