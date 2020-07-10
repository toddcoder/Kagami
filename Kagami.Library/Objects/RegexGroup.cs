using System;
using Core.Collections;
using Core.Objects;
using Core.RegularExpressions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct RegexGroup : IObject, IProcessPlaceholders, IEquatable<RegexGroup>
   {
      string text;
      int index;
      int length;
      Hash<string, IObject> passed;
      Hash<string, IObject> internals;
      Equatable<RegexGroup> equatable;

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

         equatable = new Equatable<RegexGroup>(this, "text", "index", "length");
      }

      public RegexGroup(Hash<string, IObject> passed) : this()
      {
         text = "";
         index = 0;
         length = 0;

         this.passed = passed;
         internals = new Hash<string, IObject>();

         equatable = new Equatable<RegexGroup>(this, "text", "index", "length");
      }

      public string ClassName => "Group";

      public string AsString => text;

      public string Image => $"Group({((String)text).Image}, {index}, {length})";

      public int Hash => GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is RegexGroup g && g.text == text && g.index == index && g.length == length;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => text.Length > 0;

      public String Text => text;

      public Int Index => index;

      public Int Length => length;

      public Hash<string, IObject> Passed => passed;

      public Hash<string, IObject> Internals => internals;

      public bool Equals(RegexGroup other) => equatable.Equals(other);

      public override bool Equals(object obj) => equatable.Equals(obj);

      public override int GetHashCode() => equatable.GetHashCode();
   }
}