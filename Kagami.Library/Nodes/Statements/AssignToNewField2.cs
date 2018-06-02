using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToNewField2 : Statement
   {
      bool mutable;
      Expression comparisand;
      Expression expression;

      public AssignToNewField2(bool mutable, Expression comparisand, Expression expression)
      {
         this.mutable = mutable;
         this.comparisand = comparisand;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         comparisand.Generate(builder);
         builder.Match(mutable, false, false);
         builder.Drop();
      }

      public override string ToString() => $"{(mutable ? "var" : "let")} {comparisand} = {expression}";

      public void Deconstruct(out bool mutable, out Expression comparisand, out Expression expression)
      {
         mutable = this.mutable;
         comparisand = this.comparisand;
         expression = this.expression;
      }
   }
}