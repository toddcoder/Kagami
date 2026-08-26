using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class BuilderAssign(BuilderState state, string fieldName, Expression expression, bool first) : Statement, IFieldStatement, IHasExpression
{
   protected string fieldName = fieldName;
   protected Expression expression = expression;

   public override void Generate(OperationsBuilder builder)
   {
      if (!first)
      {
         builder.GetField(state.ResultFieldName);
         builder.GoToIfFalse(state.FailureLabel, false);
      }

      var trySymbol = new TrySymbol(expression);
      trySymbol.Generate(builder);
      builder.AssignField(state.ResultFieldName, false);
      builder.UnwrapMonad();
      builder.StoreField(fieldName, false, false, true, nil);
   }

   public override string ToString() => $"let {fieldName} = {expression} [builder]";

   public string Name => fieldName;

   public bool Mutable => false;

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public Expression Expression => expression;
}