using Kagami.Library.Operations;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class AssertSymbol : Symbol
   {
      Expression condition;
      Expression value;
      IMaybe<Expression> error;

      public AssertSymbol(Expression condition, Expression value, IMaybe<Expression> error)
      {
         this.condition = condition;
         this.value = value;
         this.error = error;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var trueLabel = newLabel("true");
         var endLabel = newLabel("end");

         condition.Generate(builder);
         builder.GoToIfTrue(trueLabel);

         if (error.If(out var e))
         {
            e.Generate(builder);
            builder.Failure();
         }
         else
         {
            builder.PushNone();
         }

         builder.GoTo(endLabel);

         builder.Label(trueLabel);
         value.Generate(builder);
         if (error.IsSome)
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
         return
            $"assert{error.Map(e => "!").DefaultTo(() => "?")} {condition} : {value}{error.Map(e => $" : {e}").DefaultTo(() => "")}";
      }
   }
}