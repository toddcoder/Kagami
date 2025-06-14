using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ByteArraySymbol : Symbol
{
   protected ByteArray byteArray;

   public ByteArraySymbol(string source)
   {
      var bytes = source.Select(c => (byte)c).ToArray();
      byteArray = new ByteArray(bytes);
   }

   public override void Generate(OperationsBuilder builder) => builder.PushObject(byteArray);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => byteArray.Image;
}