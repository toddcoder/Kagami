using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class InternalList : IObject
   {
      List<IObject> list;

      public InternalList(IObject x, IObject y)
      {
         if (y is Nil)
            list = new List<IObject> { x };
         else
            list = new List<IObject> { x, y };
      }

      public InternalList(IEnumerable<IObject> objects) => list = new List<IObject>(objects);

      public IEnumerable<IObject> List => list;

      public string ClassName => "InternalList";

      public string AsString => list.Select(i => i.AsString).Listify(" ");

      public string Image => list.Select(i => i.Image).Listify();

      public int Hash => list.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is InternalList il && list.Count == il.list.Count && list.All(i => list.Contains(i));

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         return matchSingle(this, comparisand, (il, o) => il.In(o), bindings);
      }

      public bool IsTrue => list.Count > 0;

      public void Add(IObject item) => list.Add(item);

      public bool In(IObject obj) => list.Contains(obj);

      public bool ExpandInTuple { get; set; } = true;
   }
}