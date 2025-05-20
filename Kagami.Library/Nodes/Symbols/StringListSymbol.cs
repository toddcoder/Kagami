using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Strings;

namespace Kagami.Library.Nodes.Symbols;

public class StringListSymbol : Symbol
{
   protected List list = List.Empty;

   public StringListSymbol(string source)
   {
      foreach (var obj in source.Reverse().Select(KChar.CharObject))
      {
         list = List.Cons(obj, list);
      }

      list.IsString = true;
   }

   public override void Generate(OperationsBuilder builder) => builder.PushObject(list);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => list.Image;
}