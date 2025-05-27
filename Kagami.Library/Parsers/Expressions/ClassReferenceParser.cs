using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class ClassReferenceParser : SymbolParser
{
   //public override string Pattern => $"^ /(/s*) /({REGEX_CLASS_GETTING}) /b";

   public ClassReferenceParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)({REGEX_CLASS_GETTING})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Class);

      if (Module.Global.Value.Class(className) is (true, var cls))
      {
         builder.Add(new ClassSymbol(cls.Name));
         return unit;
      }
      else if (Module.Global.Value.FullDataComparisandName(className))
      {
         builder.Add(new FieldSymbol(className));
         return unit;
      }
      else if (Module.Global.Value.Forwarded(className))
      {
         builder.Add(new ClassSymbol(className));
         return unit;
      }
      else if (state.ContainsPattern(className))
      {
         builder.Add(new FieldSymbol(className));
         return unit;
      }
      else
      {
         return nil;
      }
   }
}