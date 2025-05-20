using System;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct RegexMatch : IObject, IProcessPlaceholders, IEquatable<RegexMatch>
   {
      private readonly string text;
      private readonly int index;
      private readonly int length;
      private readonly RegexGroup[] groups;
      private readonly Func<string, Maybe<int>> nameToIndex;
      private readonly Hash<string, IObject> passed;
      private readonly Hash<string, IObject> internals;

      public RegexMatch(Core.Matching.Match match, Func<string, Maybe<int>> nameToIndex) : this()
      {
         text = match.Text;
         index = match.Index;
         length = match.Length;
         groups = match.Groups.Select(g => new RegexGroup(g)).ToArray();
         this.nameToIndex = nameToIndex;

         passed = new Hash<string, IObject>();
         internals = new Hash<string, IObject>
         {
            ["text"] = (KString)text,
            ["index"] = (Int)index,
            ["length"] = (Int)length,
            ["groups"] = new KTuple(groups.Select(g => (IObject)g).ToArray())
         };
      }

      public RegexMatch(Hash<string, IObject> passed) : this()
      {
         text = "";
         index = 0;
         length = 0;
         groups = System.Array.Empty<RegexGroup>();
         nameToIndex = _ => nil;

         this.passed = passed;
         internals = new Hash<string, IObject>();
      }

      public string ClassName => "Match";

      public string AsString => text;

      public string Image => $"Match({((KString)text).Image}, {index}, {length})";

      public int Hash => GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         return obj is RegexMatch m && text == m.text && index == m.index && length == m.length && groups.Length == m.groups.Length &&
            groups.Zip(m.groups, (g1, g2) => g1.IsEqualTo(g2)).All(x => x);
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => text.Length > 0;

      public KString Text => text;

      public Int Index => index;

      public Int Length => length;

      public KTuple Groups => new(groups.Select(g => (IObject)g).ToArray());

      public Hash<string, IObject> Passed => passed;

      public Hash<string, IObject> Internals => internals;

      public KString this[int index] => index.Between(0).Until(groups.Length) ? groups[index].Text : "";

      public KString this[string name]
      {
         get
         {
            var self = this;
            return nameToIndex(name).Map(i => self[i]) | "";
         }
      }

      public bool Equals(RegexMatch other)
      {
         return text == other.text && index == other.index && length == other.length &&
            groups.Length == other.groups.Length && groups.Zip(other.groups, (g1, g2) => g1.IsEqualTo(g2)).All(x => x);
      }

      public override bool Equals(object obj) => obj is RegexMatch other && Equals(other);

      public override int GetHashCode()
      {
         return hashCode() + text.GetHashCode() + index.GetHashCode() + length.GetHashCode() + groups.GetHashCode();
      }
   }
}