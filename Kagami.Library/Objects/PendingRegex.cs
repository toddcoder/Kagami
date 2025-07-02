using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct PendingRegex(Regex regex, KString input) : IObject
{
   public Regex Regex => regex;

   public KString Input => input;

   public string ClassName => "PendingRegex";

   public string AsString => $"{input.AsString} / {regex.AsString}";

   public string Image => $"{input.Image} / {regex.Image}";

   public int Hash => input.Hash ^ regex.Hash;

   public bool IsEqualTo(IObject obj) => obj is PendingRegex other && input.IsEqualTo(other.Input) && regex.IsEqualTo(other.Regex);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => regex.Match(comparisand, bindings);

   public bool IsTrue => regex.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Replace(KString replacement) => regex.Replace(input.Value, replacement.Value);

   public IObject Replace(Lambda lambda) => regex.Replace(input.Value, lambda);
}