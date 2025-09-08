using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Cons(IObject head, IObject tail) : IObject
{
   public IObject Head => head;

   public IObject Tail => tail;

   public string ClassName => "Cons";

   public string AsString => $"{head.AsString}::{tail.AsString}";

   public string Image => $"{head.Image}::{tail.Image}";

   public int Hash => HashCode.Combine(head, tail);

   public bool IsEqualTo(IObject obj) => obj is Cons otherCons && head.IsEqualTo(otherCons.Head) && tail.IsEqualTo(otherCons.Tail);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(comparisand, comparisand, bindings);
   }

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();
}