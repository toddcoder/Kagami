using System.Linq;
using Core.Collections;
using Core.Numbers;
using Core.RegularExpressions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct RegexMatch : IObject, IProcessPlaceholders
   {
      string text;
      int index;
      int length;
      RegexGroup[] groups;
      Hash<string, IObject> passed;
      Hash<string, IObject> internals;

      public RegexMatch(Matcher.Match match) : this()
      {
         text = match.Text;
         index = match.Index;
         length = match.Length;
         groups = match.Groups.Select(g => new RegexGroup(g)).ToArray();

         passed = new Hash<string, IObject>();
         internals = new Hash<string, IObject>
         {
            ["text"] = (String)text,
            ["index"] = (Int)index,
            ["length"] = (Int)length,
            ["groups"] = new Tuple(groups.Select(g => (IObject)g).ToArray())
         };
      }

      public RegexMatch(Hash<string, IObject> passed) : this()
      {
         text = "";
         index = 0;
         length = 0;
         groups = new RegexGroup[0];

         this.passed = passed;
         internals = new Hash<string, IObject>();
      }

      public string ClassName => "Match";

      public string AsString => text;

      public string Image => $"Match({((String)text).Image}, {index}, {length})";

      public int Hash
      {
         get
         {
            var hash = 17;
            hash = 37 * hash + text.GetHashCode();
            hash = 37 * hash + index.GetHashCode();
            hash = 37 * hash + length.GetHashCode();

            foreach (var group in groups)
            {
	            hash = 37 * hash + group.Hash;
            }

            return hash;
         }
      }

      public bool IsEqualTo(IObject obj)
      {
         return obj is RegexMatch m && text == m.text && index == m.index && length == m.length && groups.Length == m.groups.Length &&
            groups.Zip(m.groups, (g1, g2) => g1.IsEqualTo(g2)).All(x => x);
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => text.Length > 0;

      public String Text => text;

      public Int Index => index;

      public Int Length => length;

      public Tuple Groups => new Tuple(groups.Select(g => (IObject)g).ToArray());

      public Hash<string, IObject> Passed => passed;

      public Hash<string, IObject> Internals => internals;

      public String this[int index]
      {
	      get
	      {
		      if (index.Between(0).Until(groups.Length))
		      {
			      return groups[index].Text;
		      }
		      else
		      {
			      return "";
		      }
	      }
      }
   }
}