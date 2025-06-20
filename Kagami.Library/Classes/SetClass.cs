﻿using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class SetClass : BaseClass, ICollectionClass
{
   public override string Name => "Set";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();

      messages["<<(_)"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, i) => s.Append(i));
      messages[">>(_)"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, i) => s.Remove(i));
      messages["remove(_)"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, i) => s.Remove(i));
      messages["+(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Union(s2));
      messages["-(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Difference(s2));
      messages["*(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Intersection(s2));
      messages["/(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.XOr(s2));
      messages["[](_)"] = (obj, msg) => function<Set, Int>(obj, msg, (s, i) => s[i.Value]);
      messages["length".get()] = (obj, _) => function<Set>(obj, s => s.Length);
      messages["extend()"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, _) => s.Extend());
      messages["clear()"] = (obj, _) => function<Set>(obj, s => s.Clear());
      messages["classify(_)"] = (obj, msg) => function<Set, Lambda>(obj, msg, (s, l) => s.Classify(l));
   }

   public IObject Revert(IEnumerable<IObject> list) => new Set(list.ToArray());

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}