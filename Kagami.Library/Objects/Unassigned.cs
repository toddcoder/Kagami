using Standard.Types.Collections;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Objects
{
   public struct Unassigned : IObject
   {
      public static IObject Value => new Unassigned();

      public string ClassName => "Unassigned";

      public string AsString => throw unassigned();

      public string Image => "Unassigned";

      public int Hash => ClassName.GetHashCode();

      public bool IsEqualTo(IObject obj) => throw unassigned();

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => throw unassigned();

	   public bool IsTrue => throw unassigned();
   }
}