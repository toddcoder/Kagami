using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Statements;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers.Expressions;

public partial class AnonymousBuilderParser(ExpressionBuilder builder) : SymbolParser(builder)
{
   [GeneratedRegex(@"^(\s*)(do)\b(?=\s*\{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var defaultExpression = new Expression(new PushObjectSymbol(new Objects.Failure("No value")));
      var builderState = new BuilderState("value", newLabel("failure"), true);
      state.PushStatements();

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _scan = state.BeginBlock();
      if (!_scan)
      {
         return _scan.Exception;
      }

      while (state.More)
      {
         _scan = state.EndBlock();
         if (_scan)
         {
            break;
         }
         else if (_scan.Exception is (true, var exception))
         {
            return exception;
         }

         var builderMembersParser = new BuilderMembersParser(builderState);
         _scan = builderMembersParser.Scan(state);
         if (_scan)
         {
            if (builderState.First)
            {
               builderState = builderState with { First = false };
            }
         }
         else
         {
            return _scan.Exception;
         }
      }

      state.RemoveYieldFlag();
      state.RemoveReturnType();

      var _statements = state.PopStatements();
      if (_statements is (true, var statements))
      {
         Block block = [new AssignToNewField(true, builderState.ResultFieldName, defaultExpression, false, false)];
         foreach (var statement in statements)
         {
            block.Add(statement);
            if (statement is BuilderReturn)
            {
               break;
            }
         }

         block.Add(new Return(new Expression(new FieldSymbol(builderState.ResultFieldName)), nil));
         builder.Add(new BuilderSymbol(block));
         return unit;
      }
      else
      {
         return _statements.Exception;
      }
   }
}