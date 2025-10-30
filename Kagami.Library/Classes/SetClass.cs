using Core.Monads;
using Kagami.Library.Objects;
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
      messages["remove(_)"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, i) => s.RemoveAndReturn(i));
      messages["+(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Union(s2));
      messages["union(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Union(s2));
      messages["-(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Difference(s2));
      messages["difference(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Difference(s2));
      messages["*(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Intersection(s2));
      messages["intersection(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Intersection(s2));
      messages["/(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.XOr(s2));
      messages["symmetricDifference(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.XOr(s2));
      messages["[](_)"] = (obj, msg) => function<Set, Int>(obj, msg, (s, i) => s[i.Value]);
      messages["length".get()] = (obj, _) => function<Set>(obj, s => s.Length);
      messages["clear()"] = (obj, _) => function<Set>(obj, s => s.Clear());
      messages["classify(_)"] = (obj, msg) => function<Set, Lambda>(obj, msg, (s, l) => s.Classify(l));
      messages["~(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s, l) => s.Concatenate(l));
      messages["isSubsetOf(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.IsSubsetOf(s2));
      messages["isProperSubsetOf(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.IsProperSubsetOf(s2));
      messages["isSupersetOf(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.IsSupersetOf(s2));
      messages["isProperSupersetOf(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.IsProperSupersetOf(s2));
      messages["overlaps(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.Overlaps(s2));
      messages["isDisjointWith(_)"] = (obj, msg) => function<Set, Set>(obj, msg, (s1, s2) => s1.IsDisjointWith(s2));
      messages["extend(_)"] = (obj, msg) => function<Set, IObject>(obj, msg, (s, o) => s.Extend(o));
   }

   public override IObject DefaultValue => Set.Empty;

   public IObject Revert(IEnumerable<IObject> list, Maybe<TypeConstraint> _typeConstraint) => new Set((IObject[])[.. list]);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["clrf".get()] = (_, _) => getSet("\r\n");
      classMessages["lalpha".get()] = (_, _) => getSet("abcdefghijklmnopqrstuvwxyz");
      classMessages["ualpha".get()] = (_, _) => getSet("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
      classMessages["alpha".get()] = (_, _) => getSet("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
      classMessages["lvowels".get()] = (_, _) => getSet("aeiou");
      classMessages["uvowels".get()] = (_, _) => getSet("AEIOU");
      classMessages["lconsonants".get()] = (_, _) => getSet("bcdfghjklmnpqrstvwxyz");
      classMessages["uconsonants".get()] = (_, _) => getSet("BCDFGHJKLMNPQRSTVWXYZ");
      classMessages["digits".get()] = (_, _) => getSet("0123456789");
      classMessages["punctuation".get()] = (_, _) => getSet("!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~");

      return;

      static Set getSet(string @string) => new((IObject[])[.. @string.ToCharArray().Select(KChar.CharObject)]);
   }
}