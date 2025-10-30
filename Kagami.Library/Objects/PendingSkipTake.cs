using Core.Collections;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct PendingSkipTake(ISkipTakeable skipTakeable, SkipTake skipTake) : IObject
{
   public IObject SkipTakableObject => (IObject)skipTakeable;

   public SkipTake SkipTake => skipTake;

   public string ClassName => "PendingSkipTake";

   public string AsString => $"{SkipTakableObject.AsString} {skipTake.AsString}";

   public string Image => $"{SkipTakableObject.Image} {skipTake.Image}";

   public int Hash => HashCode.Combine(skipTakeable, skipTake);

   public bool IsEqualTo(IObject obj)
   {
      return obj is PendingSkipTake otherPendingSkipTake &&
         SkipTakableObject.IsEqualTo(otherPendingSkipTake.SkipTakableObject) && skipTake.IsEqualTo(otherPendingSkipTake.SkipTake);
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => skipTake.Skip > 0 || skipTake.Take > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Replace(ICollection collection)
   {
      List<IObject> result = [];
      List<IObject> source = [.. ((ICollection)skipTakeable).GetIterator(false).List()];
      List<IObject> left = [.. source.Take(skipTake.Skip)];
      List<IObject> middle = [.. collection.GetIterator(false).List()];
      List<IObject> right = [.. source.Skip(skipTake.Skip + skipTake.Take)];

      result.AddRange(left);
      result.AddRange(middle);
      result.AddRange(right);

      return Module.CollectionClass((ICollection)skipTakeable).Revert(result, nil);
   }
}