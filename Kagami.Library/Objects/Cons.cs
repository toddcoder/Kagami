using Core.Collections;
using Core.Monads;

namespace Kagami.Library.Objects;

public class Cons(IObject head, IObject tail) : IObject
{
   public static IObject Combine(IObject head, IObject tail) => tail switch
   {
      Placeholder => new Cons(head, tail),
      KArray { IsNotEmpty.Value: true } array => new Cons(head, new KArray(head).Concatenate(array)),
      KArray => new Cons(head, tail),
      Cons cons => new Cons(head, cons.ToArray()),
      _ => new Cons(head, tail)
   };

   public IObject Head => head;

   public IObject Tail => tail;

   public string ClassName => "Cons";

   public string AsString => $"{head.AsString}::{tail.AsString}";

   public string Image => $"({head.Image}::{tail.Image})";

   public int Hash => HashCode.Combine(head, tail);

   public bool IsEqualTo(IObject obj) => obj is Cons otherCons && head.IsEqualTo(otherCons.Head) && tail.IsEqualTo(otherCons.Tail);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => comparisand switch
   {
      Cons cons => cons.Head.Match(head, bindings) && cons.Tail.Match(tail, bindings),
      KArray { IsNotEmpty.Value: true } array => head.Match(array[0], bindings) && ((Cons)tail).ToArray().Match(array.Tail, bindings),
      _ => false
   };

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IEnumerable<IObject> Enumerable()
   {
      yield return head;

      if (tail is ICollection collection)
      {
         var iterator = collection.GetIterator(false);
         while (iterator.Next() is (true, var item))
         {
            yield return item;
         }
      }
      else
      {
         yield return tail;
      }
   }

   public KArray ToArray() => new(Enumerable());

   public KArray ToArray(Maybe<TypeConstraint> typeConstraint) => new(Enumerable()) { TypeConstraint = typeConstraint };
}