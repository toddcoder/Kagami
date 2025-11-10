using Core.Collections;

namespace Kagami.Library.Objects;

public record Singleton(string Name, TypeConstraint TypeConstraint) : IObject
{
   public string ClassName => "Singleton";

   public string AsString => $"{Name} {TypeConstraint.AsString}";

   public string Image => $"{Name} {TypeConstraint.Image}";

   public int Hash => HashCode.Combine(Name, TypeConstraint);

   public bool IsEqualTo(IObject obj)
   {
      return obj is Singleton otherSingleton && Name == otherSingleton.Name && TypeConstraint.IsEqualTo(otherSingleton.TypeConstraint);
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => IsEqualTo(comparisand);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public string FunctionName => $"__${Name}_{TypeConstraint.AsString}";
}