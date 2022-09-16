using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class MutStringClass : BaseClass, ICollectionClass
   {
      public override string Name => "MutString";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();
         sliceableMessages();
         compareMessages();
         rangeMessages();
         textFindingMessages();
         mutableCollectionMessages();

         registerMessage("<<", (obj, msg) => function<MutString, IObject>(obj, msg, (m, o) => m.Append(o)));
         registerMessage("[](_<Int>)", (obj, msg) => function<MutString, Int>(obj, msg, (m, i) => m[i.Value]));
         registerMessage("[]=(_<Int>,_<Char>)", (obj, msg) => function<MutString, Int, Char>(obj, msg, (m, i, v) => m[i.Value] = v));
         registerMessage("fill(char:_<Char>,count:_<Int>)",
            (obj, msg) => function<MutString, Char, Int>(obj, msg, (m, c, i) => m.Fill(c.Value, i.Value)));
         registerMessage("fill(count:_<Int>,char:_<Char>)",
            (obj, msg) => function<MutString, Int, Char>(obj, msg, (m, i, c) => m.Fill(c.Value, i.Value)));
      }

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection", "TextFinding");

      public IObject Revert(IEnumerable<IObject> list) => (MutString)list.Select(i => i.AsString).ToString("");
   }
}