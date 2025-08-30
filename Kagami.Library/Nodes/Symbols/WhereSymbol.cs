using Core.Enumerables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WhereSymbol(Expression expression, TaggedExpression[] taggedExpressions) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFrame();

      foreach (var (tag, exp) in taggedExpressions)
      {
         builder.NewField(tag, false, true);
         exp.Generate(builder);
         builder.AssignField(tag, false);
      }

      expression.Generate(builder);

      builder.PopFrameWithValue();
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"{expression} where ({taggedExpressions.Select(t => $"{t.Tag}={t.Expression}").ToString(", ")})";
}