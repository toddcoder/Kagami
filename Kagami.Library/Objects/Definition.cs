using Core.Collections;

namespace Kagami.Library.Objects;

public struct Definition(Lambda lambda) : IObject
{
   public Lambda Lambda => lambda;

   public string ClassName => "Definition";

   public string AsString => lambda.Image;

   public string Image => lambda.Image;

   public int Hash => lambda.Hash;

   public bool IsEqualTo(IObject obj) => obj is Definition other && lambda.IsEqualTo(other.Lambda);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();
}