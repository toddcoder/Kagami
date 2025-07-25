using Core.Collections;
using Core.Strings;
using static Core.Objects.GetHashCodeGenerator;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct RegexGroup : IObject, IProcessPlaceholders, IEquatable<RegexGroup>
{
   private readonly string text;
   private readonly int index;
   private readonly int length;
   private readonly Hash<string, IObject> passed;
   private readonly Hash<string, IObject> internals;
   private readonly string prefix;
   private readonly string suffix;

   public RegexGroup(Core.Matching.Group group, string input) : this()
   {
      var (groupText, groupIndex, groupLength) = group;
      text = groupText;
      index = groupIndex;
      length = groupLength;

      passed = new Hash<string, IObject>();
      prefix = input.Keep(index);
      suffix = input.Drop(index + length);
      internals = new Hash<string, IObject>
      {
         ["text"] = (KString)text,
         ["index"] = (Int)index,
         ["length"] = (Int)length,
         ["prefix"] = (KString)prefix,
         ["suffix"] = (KString)suffix
      };
   }

   public RegexGroup(Hash<string, IObject> passed) : this()
   {
      text = "";
      index = 0;
      length = 0;
      prefix = "";
      suffix = "";

      this.passed = passed;
      internals = new Hash<string, IObject>();
   }

   public string ClassName => "Group";

   public string AsString => text;

   public string Image => $"Group({((KString)text).Image}, {index}, {length})";

   public int Hash => GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is RegexGroup g && g.text == text && g.index == index && g.length == length;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => text.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public KString Text => text;

   public Int Index => index;

   public Int Length => length;

   public Hash<string, IObject> Passed => passed;

   public Hash<string, IObject> Internals => internals;

   public KString Prefix => prefix;

   public KString Suffix => suffix;

   public bool Equals(RegexGroup other) => text == other.text && index == other.index && length == other.length;

   public override bool Equals(object? obj) => obj is RegexGroup other && Equals(other);

   public override int GetHashCode() => hashCode() + text.GetHashCode() + index.GetHashCode() + length.GetHashCode();
}