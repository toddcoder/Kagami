using Core.Collections;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Error : IObject
{
   private readonly string message;

   public Error(string message) : this() => this.message = message;

   public string ClassName => "Error";

   public string AsString => $"Error: {message}";

   public string Image => $"Error({message})";

   public int Hash => message.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Error e && message == e.message;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => message.IsNotEmpty();

   public Guid Id { get; init; } = Guid.NewGuid();

   public KString Message => message;
}