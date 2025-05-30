namespace Kagami.Library.Objects;

public interface IIndexed
{
   IObject this[int index] { get; set; }
}