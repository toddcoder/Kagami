namespace Kagami.Library.Objects;

public interface IIndexed
{
   IObject this[int index] { get; set; }

   IObject this[KRange range] { get; set; }

   int LastIndex { get; }

   int Length { get; }

   KRange Start { get; }

   KRange End { get; }

   KRange Indexes { get; }
}