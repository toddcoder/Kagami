using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class RemainderZeroSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.Remainder();
         builder.PushInt(0);
         builder.Equal();
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "%%";
   }
}