using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class RemainderZeroSymbol : Symbol
   {
      protected bool not;

      public RemainderZeroSymbol(bool not) => this.not = not;

      public override void Generate(OperationsBuilder builder)
      {
         builder.Remainder();
         builder.PushInt(0);
         builder.Equal();
         if (not)
         {
            builder.Not();
         }
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => not ? "!%" : "%%";
   }
}