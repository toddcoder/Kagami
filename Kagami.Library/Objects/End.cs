using Standard.Types.Collections;
using Standard.Types.Exceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct End : IObject, IObjectCompare
   {
      public static IObject Value => new End();

      public string ClassName => "End";

      public string AsString => "end";

      public string Image => "end";

      public int Hash => "end".GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is End;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => false;

      public int Compare(IObject obj) => throw "End not replaced".Throws();
   }
}