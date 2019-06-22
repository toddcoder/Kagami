using System.Linq;
using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct DataComparisand : IObject
   {
      string className;
      string name;
      IObject[] comparisands;
      IObject ordinal;

      public DataComparisand(string className, string name, IObject[] comparisands, IObject ordinal) : this()
      {
         this.className = className;
         this.name = name;
         this.comparisands = comparisands;
         this.ordinal = ordinal;
      }

      public String Name => name;

      public Tuple Comparisands => new Tuple(comparisands);

      public IObject Ordinal => ordinal;

      public string ClassName => $"{className}.{name}";

      public string AsString => name;

      public string Image
      {
         get
         {
            if (comparisands.Length == 0)
            {
	            return name;
            }
            else
            {
	            return $"{name}({comparisands.Select(c => c.Image).Stringify()})";
            }
         }
      }

      public int Hash => comparisands.GetHashCode();

      bool comparisandsEqual(IObject[] others)
      {
         if (comparisands.Length == others.Length)
         {
            var self = this;
            return Enumerable.Range(0, comparisands.Length).Select(i => self.comparisands[i].IsEqualTo(others[i])).All(b => b);
         }
         else
         {
	         return false;
         }
      }

      public bool IsEqualTo(IObject obj)
      {
         return obj is DataComparisand dc && className == dc.className && ordinal.IsEqualTo(dc.ordinal) && comparisandsEqual(dc.comparisands);
      }

      bool comparisandsMatch(IObject[] others, Hash<string, IObject> bindings)
      {
         var self = this;
         return Enumerable.Range(0, comparisands.Length).Select(i => self.comparisands[i].Match(others[i], bindings)).All(b => b);
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         return match(this, comparisand, (dc1, dc2) => dc1.comparisandsMatch(dc2.comparisands, bindings), bindings);
      }

      public bool IsTrue => true;
   }
}