using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class ClassReferenceParser : SymbolParser
{
   public static Optional<Unit> AddClassReference(ParseState state, ExpressionBuilder builder, string className)
   {
      if (state.IsPattern(className))
      {
         builder.Add(new FieldSymbol(className));
      }

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
      else if (Module.IsBuiltInClass(className))
      {
         builder.Add(new FieldSymbol(className));
         return unit;
      }
      else if (Protocols.Protocols.Get(className))
      {
         builder.Add(new PushObjectSymbol(new ProtocolConstraint(className)));
         return unit;
      }
      else
      {
         return nil;
      }
   }

   public ClassReferenceParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)({REGEX_CLASS_GETTING})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Class);

      return AddClassReference(state, builder, className);
   }
}