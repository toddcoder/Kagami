using System.Text.RegularExpressions;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Monads.Lazy;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class TypeConstraintParser : SymbolParser
{
   public TypeConstraintParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /'<' (> ['A-Z'])";

   [GeneratedRegex(@"^(\s*)(<)(?=[A-Z])", RegexOptions.Compiled)]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Class);

      List<BaseClass> list = [];
      while (state.More)
      {
         LazyOptional<string> _scan = nil;
         var _name = state.Scan($"^ /(/s*) /({REGEX_CLASS})", Color.Whitespace, Color.Class);
         if (_name is (true, var name))
         {
            name = name.TrimStart();
            if (Module.Global.Value.Class(name) is (true, var baseClass))
            {
               list.Add(baseClass);
            }
            else if (Module.Global.Value.Forwarded(name))
            {
               list.Add(new ForwardedClass(name));
            }
            else
            {
               return classNotFound(name);
            }
         }
         else if (_name.Exception is (true, var exception))
         {
            return exception;
         }
         else if (_scan.ValueOf(state.Scan("^ /'>'", Color.Class)))
         {
            builder.Add(new TypeConstraintSymbol(list));
            return unit;
         }
         else if (_scan.Exception is (true, var exception2))
         {
            return exception2;
         }
         else
         {
            return fail("Open type constraint");
         }
      }

      return unit;
   }
}