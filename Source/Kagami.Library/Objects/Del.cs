using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Del : IObject
   {
      public static IObject Value => new Del();

      public string ClassName => "Del";

      public string AsString => "del";

      public string Image => "del";

      public int Hash => "del".GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Del;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => false;
   }
}