using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Nodes
{
   public static class NodeFunctions
   {
      static int uniqueID;

      public static string newLabel(string name) => mangled(name, uniqueID++);

      public static void ResetUniqueID() => uniqueID = 0;

      public static string id() => uniqueID++.ToString();
   }
}