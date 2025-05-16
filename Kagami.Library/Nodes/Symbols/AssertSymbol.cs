using Kagami.Library.Operations;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class AssertSymbol : Symbol
{
   protected Expression condition;
   protected Expression value;
   protected Maybe<Expression> _error;

   public AssertSymbol(Expression condition, Expression value, Maybe<Expression> _error)
   {
      this.condition = condition;
      this.value = value;
      this._error = _error;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var trueLabel = newLabel("true");
      var endLabel = newLabel("end");

      condition.Generate(builder);
      builder.GoToIfTrue(trueLabel);

      if (_error is (true, var error))
      {
         error.Generate(builder);
         builder.Failure();
      }
      else
      {
         builder.PushNone();
      }

      builder.GoTo(endLabel);

      builder.Label(trueLabel);
      value.Generate(builder);
      if (_error)
      {
         builder.Success();
      }
      else
      {
         builder.Some();
      }

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Nullary;

   public override string ToString()
   {
      return $"assert{_error.Map(_ => "!")|(() => "?")} {condition} : {value}{_error.Map(e => $" : {e}") | (() => "")}";
   }
}