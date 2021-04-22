using System;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.RegularExpressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct RegexMatch : IObject, IProcessPlaceholders, IEquatable<RegexMatch>
   {
      private readonly string text;
      private readonly int index;
      private readonly int length;
      private readonly RegexGroup[] groups;
      private readonly Func<string, IMaybe<int>> nameToIndex;
      private readonly Hash<string, IObject> passed;
      private readonly Hash<string, IObject> internals;
      private readonly Equatable<RegexMatch> equatable;

      public RegexMatch(Matcher.Match match, Func<string, IMaybe<int>> nameToIndex) : this()
      {
         text = match.Text;
         index = match.Index;
         length = match.Length;
         groups = match.Groups.Select(g => new RegexGroup(g)).ToArray();
         this.nameToIndex = nameToIndex;

         passed = new Hash<string, IObject>();
         internals = new Hash<string, IObject>
         {
            ["text"] = (String)text,
            ["index"] = (Int)index,
            ["length"] = (Int)length,
            ["groups"] = new Tuple(groups.Select(g => (IObject)g).ToArray())
         };

         equatable = new Equatable<RegexMatch>(this, "text", "index", "length", "groups");
      }

      public RegexMatch(Hash<string, IObject> passed) : this()
      {
         text = "";
         index = 0;
         length = 0;
         groups = new RegexGroup[0];
         nameToIndex = _ => none<int>();

         this.passed = passed;
         internals = new Hash<string, IObject>();

         equatable = new Equatable<RegexMatch>(this, "text", "index", "length", "groups");
      }

      public string ClassName => "Match";

      public string AsString => text;

      public string Image => $"Match({((String)text).Image}, {index}, {length})";

      public int Hash => GetHashCode();

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

      public Tuple Groups => new(groups.Select(g => (IObject)g).ToArray());

      public Hash<string, IObject> Passed => passed;

      public Hash<string, IObject> Internals => internals;

      public String this[int index] => index.Between(0).Until(groups.Length) ? groups[index].Text : "";

      public String this[string name] => nameToIndex(name).If(out var i) ? this[i] : "";

      public bool Equals(RegexMatch other) => equatable.Equals(other);

      public override bool Equals(object obj) => obj is RegexMatch other && Equals(other);

      public override int GetHashCode() => equatable.GetHashCode();
   }
}