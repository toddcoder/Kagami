using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using EndOfLine = Kagami.Library.Nodes.Statements.EndOfLine;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers.Expressions;

public partial class BuilderParser : SymbolParser
{
   public BuilderParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@$"^(\s*)(do)(\s+)({REGEX_CLASS})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var className = tokens[4].Text;
      var hasArguments = tokens[5].Text == "(";
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

      var arguments = hasArguments ? getArguments(state, builder.Flags) | (() => []) : [];

      var invoke = new InvokeSymbol(className, arguments, nil, false);
      builder.Add(invoke);

      var _scanned = state.BeginBlock();
      if (!_scanned)
      {
         return _scanned.Exception;
      }

      builder.Add(new PushFrameSymbol(true));

      state.PushStatements();

      var failureLabel = newLabel("failure");

      while (state.More)
      {
         _scanned = state.EndBlock();
         if (_scanned)
         {
            break;
         }
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }

         var builderStatementsParser = new BuilderStatementsParser();
         _scanned = builderStatementsParser.Scan(state);
         if (_scanned)
         {
         }
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      var _statements = state.PopStatements();
      if (_statements is (true, var statements))
      {
         foreach (var statement in statements)
         {
            switch (statement)
            {
               case AssignToField assignToField:
               {
                  var name = assignToField.Name;
                  var expression = assignToField.Expression;
                  builder.Add(new BuilderAssignSymbol(name, expression, failureLabel));
                  break;
               }
               case Return @return:
               {
                  var expression = @return.Expression;
                  builder.Add(new BuilderReturnSymbol(expression, failureLabel));
                  break;
               }
               case EndOfLine:
                  break;
               default:
                  return fail($"Unexpected statement: {statement}");
            }
         }
      }
      else
      {
         return _statements.Exception;
      }

      builder.Add(new PopFrameSymbol());

      return unit;
   }
}