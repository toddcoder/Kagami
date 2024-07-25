namespace Kagami.Library.Objects
{
   public interface IOptional
   {
      IObject Value { get; }

      bool IsSome { get; }

      bool IsNone { get; }

      IObject Map(Lambda lambda);

      IObject FlatMap(Lambda ifSome, Lambda ifNone);
   }
}