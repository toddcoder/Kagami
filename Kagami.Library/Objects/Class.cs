using Core.Collections;
using Kagami.Library.Classes;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Class : IObject, IEquatable<Class>
{
   private readonly string className;

   public Class(string className) : this() => this.className = className;

   public string ClassName => className;

   public string AsString => className;

   public string Image => className;

   public int Hash => className.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Class c && className == c.className;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Class other) => className == other.className;

   public override bool Equals(object? obj) => obj is Class other && Equals(other);

   public override int GetHashCode() => className.GetHashCode();

   public static bool operator ==(Class left, Class right) => left.Equals(right);

   public static bool operator !=(Class left, Class right) => !left.Equals(right);
}