using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct Binding : IObject
{
   private readonly string name;
   private readonly IObject value;

   public Binding(string name, IObject value) : this()
   {
      this.name = name;
      this.value = value;
   }

   public string Name => name;

   public IObject Value => value;

   public string ClassName => "Binding";

   public string AsString => value.AsString;

   public string Image => $"{name}' {value.Image}";

   public int Hash => Image.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Binding b && name == b.name && value.IsEqualTo(b.value);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => value.Match(comparisand, bindings);

   public bool IsTrue => value.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();
}