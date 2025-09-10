using Core.Enumerables;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

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
      registerMessage("[]=(_<Int>,_<Char>)", (obj, msg) => function<MutString, Int, KChar>(obj, msg, (m, i, v) => m[i.Value] = v));
      registerMessage("fill(char:_<Char>,count:_<Int>)",
         (obj, msg) => function<MutString, KChar, Int>(obj, msg, (m, c, i) => m.Fill(c.Value, i.Value)));
      registerMessage("fill(count:_<Int>,char:_<Char>)",
         (obj, msg) => function<MutString, Int, KChar>(obj, msg, (m, i, c) => m.Fill(c.Value, i.Value)));
      registerMessage("pop()", (obj, _) => function<MutString>(obj, m => m.Pop()));
      registerMessage("dequeue()", (obj, _) => function<MutString>(obj, m => m.Dequeue()));
      registerMessage("print(_<String>)", (obj, msg) => function<MutString, KString>(obj, msg, (m, s) => m.Print(s.Value)));
      registerMessage("println(_<String>)", (obj, msg) => function<MutString, KString>(obj, msg, (m, s) => m.PrintLine(s.Value)));
      registerMessage("put(_<String>)", (obj, msg) => function<MutString, KString>(obj, msg, (m, s) => m.Put(s.Value)));
      registerMessage("put(_<String>,_<String>)",
         (obj, msg) => function<MutString, KString, KString>(obj, msg, (m, s1, s2) => m.Put(s1.Value, s2.Value)));
   }

   public override IObject DefaultValue => new MutString("");

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection", "TextFinding");

   public IObject Revert(IEnumerable<IObject> list) => (MutString)list.Select(i => i.AsString).ToString("");
}