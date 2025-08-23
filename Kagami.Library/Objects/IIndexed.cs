namespace Kagami.Library.Objects;

public interface IIndexed
{
   IObject this[int index] { get; set; }

   IObject this[KIndex index] { get; set; }

   int LastIndex { get; }

   int Length { get; }

   KIndex Start { get; }

   KIndex End { get; }

   KIndex Full { get; }
}