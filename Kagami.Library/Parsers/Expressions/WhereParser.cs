using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using System.Text.RegularExpressions;

namespace Kagami.Library.Parsers.Expressions;

public partial class WhereParser : SymbolParser
{
   public WhereParser(ExpressionBuilder builder) : base(builder)
   {
   }

   //public override string Pattern => "^ /(/s*) /'.{'";

   [GeneratedRegex(@"^(\s*)(\.{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Structure);

      var itemParser = new WhereItemParser(builder);
      List<(string, Expression)> list = [];

      while (state.More)
      {
         var _scan = itemParser.Scan(state);
         if (_scan)
         {
            list.Add((itemParser.PropertyName, itemParser.Expression));
            if (state.Scan("^ /(/s*) /[',}']", Color.Whitespace, Color.Structure) is (true, var value))
            {
               if (value.Trim() == "}")
               {
                  builder.Add(new WhereSymbol([.. list]));
                  return unit;
               }
            }
            else
            {
               return fail("Open where");
            }
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return nil;
         }
      }

      return nil;
   }
}