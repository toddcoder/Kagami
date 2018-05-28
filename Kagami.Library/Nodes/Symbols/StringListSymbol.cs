using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Strings;

namespace Kagami.Library.Nodes.Symbols
{
   public class StringListSymbol : Symbol
   {
      List list;

      public StringListSymbol(string source)
      {
         list = List.Empty;
         foreach (var ch in source.Reverse())
         {
            var obj = Char.CharObject(ch);
            list = List.Cons(obj, list);
         }

         list.IsString = true;
      }

      public override void Generate(OperationsBuilder builder) => builder.PushObject(list);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => list.Image;
   }
}