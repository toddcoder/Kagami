using Core.Collections;

namespace Kagami.Library.Objects
{
   public struct Any : IObject
   {
      public static IObject Value => new Any();

      public string ClassName => "Any";

      public string AsString => "_";

      public string Image => "_";

      public int Hash => "_".GetHashCode();

      public bool IsEqualTo(IObject obj) => true;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => true;

	   public bool IsTrue => true;
   }
}