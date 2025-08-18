using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class InitializerParser : SymbolParser
{
   protected bool standAlone;

   public InitializerParser(ExpressionBuilder builder, bool standAlone = true) : base(builder)
   {
      this.standAlone = standAlone;
   }

   [GeneratedRegex(@"^(\s*)(\.{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Block);

      List<(string, Expression)> properties = [];

      while (state.More)
      {
         var _result =
            from propertyName in state.Scan(@$"^(\s*)({REGEX_FIELD})(\s*)(=)", 2, Color.Whitespace, Color.Message, Color.Whitespace, Color.Structure)
            from expression in getExpression(state, builder.Flags | ExpressionFlags.OmitAssign | ExpressionFlags.OmitComma)
            select (propertyName, expression);
         if (_result is (true, var property))
         {
            properties.Add(property);
         }
         else
         {
            return _result.Exception;
         }

         var _terminal = state.Scan(@"^(\s*)([},])", (g, i) => i switch
         {
            1 => Color.Whitespace,
            2 when g.Value == "," => Color.Structure,
            2 when g.Value == "}" => Color.Block,
            _ => Color.Whitespace
         });
         if (_terminal is (true, var terminal))
         {
            if (terminal == "}")
            {
               break;
            }
         }
         else
         {
            return _terminal.Exception;
         }
      }

      if (standAlone)
      {
         builder.Add(new InitializerSymbol(properties));
      }
      else
      {
         Properties = properties;
      }

      return unit;
   }

   public List<(string, Expression)> Properties { get; set; } = [];
}