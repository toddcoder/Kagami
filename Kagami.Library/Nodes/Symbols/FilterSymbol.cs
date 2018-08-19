using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class FilterSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder)
      {
         builder.SendMessage("if()", 1);
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "&";
   }
}