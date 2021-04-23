using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SliceAssignSymbol : Symbol
   {
      protected Expression indexes;
      protected Expression values;

      public SliceAssignSymbol(Expression indexes, Expression values)
      {
         this.indexes = indexes;
         this.values = values;
      }

      public override void Generate(OperationsBuilder builder)
      {
         indexes.Generate(builder);
         values.Generate(builder);

         builder.SendMessage("assign(_,_)", 2);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"{{{indexes}}} = {values}";
   }
}