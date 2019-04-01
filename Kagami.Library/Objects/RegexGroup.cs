using Core.Collections;
using Core.RegularExpressions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct RegexGroup : IObject, IProcessPlaceholders
   {
      string text;
      int index;
      int length;
      Hash<string, IObject> passed;
      Hash<string, IObject> internals;

      public RegexGroup(Matcher.Group group) : this()
      {
         text = group.Text;
         index = group.Index;
         length = group.Length;

         passed = new Hash<string, IObject>();
         internals = new Hash<string, IObject>
         {
            ["text"] = (String)text,
            ["index"] = (Int)index,
            ["length"] = (Int)length
         };
      }

      public RegexGroup(Hash<string, IObject> passed)
      {
         text = "";
         index = 0;
         length = 0;

         this.passed = passed;
         internals = new Hash<string, IObject>();
      }

      public string ClassName => "Group";

      public string AsString => text;

      public string Image => $"Group({((String)text).Image}, {index}, {length})";

      public int Hash
      {
         get
         {
            var hash = 17;
            hash = 37 * hash + text.GetHashCode();
            hash = 37 * hash + index.GetHashCode();
            hash = 37 * hash + length.GetHashCode();

            return hash;
         }
      }

      public bool IsEqualTo(IObject obj) => obj is RegexGroup g && g.text == text && g.index == index && g.length == length;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => text.Length > 0;

      public String Text => text;

      public Int Index => index;

      public Int Length => length;

      public Hash<string, IObject> Passed => passed;

      public Hash<string, IObject> Internals => internals;
   }
}