using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using Core.Monads.Lazy;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DeferParser : StatementParser
{
   [GeneratedRegex(@"^(\s+)(defer)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      Block block;
      LazyOptional<Expression> _expression = nil;
      if (state.CurrentSource.IsMatch("/s* '{'"))
      {
         var _block = getBlock(state);
         if (_block)
         {
            block = _block;
         }
         else
         {
            return _block.Exception;
         }
      }
      else if (_expression.ValueOf(getExpression(state, ExpressionFlags.Standard)) is (true, var expression))
      {
         block = new Block(new ExpressionStatement(expression, true));
      }
      else
      {
         return _expression.Exception;
      }

      var functionName = $"__$deferred{Guid.NewGuid()}";
      var function = new Function(functionName, Parameters.Empty, block, false, false, "");
      state.RegisterMacro(function);

      block.AddReturnIf();
      state.AddStatement(new Defer(block));

      return unit;
   }
}