using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class FormatSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.SendMessage("format()", 1);

      public override Precedence Precedence => Precedence.Format;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => @"\\";
   }
}