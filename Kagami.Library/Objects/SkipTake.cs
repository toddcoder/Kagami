using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct SkipTake : IObject, IEquatable<SkipTake>
{
   private int skip;
   private int take;

   public SkipTake(int skip, int take) : this()
   {
      this.skip = skip;
      this.take = take;
   }

   public string ClassName => "SkipTake";

   public string AsString => $"{skip}:{take}";

   public string Image => AsString;

   public int Hash => (skip.GetHashCode() + take.GetHashCode()).GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is SkipTake skipTake && Equals(skipTake);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => skip != 0 && take != 0;

   public int Skip => skip;

   public int Take => take;

   public bool Equals(SkipTake other) => skip == other.Skip && take == other.Take;

   public override bool Equals(object? obj) => obj is SkipTake other && Equals(other);

   public override int GetHashCode() => Hash;

   public void Deconstruct(out int skip, out int take)
   {
      skip = this.skip;
      take = this.take;
   }

   public bool NoTake { get; set; }
}