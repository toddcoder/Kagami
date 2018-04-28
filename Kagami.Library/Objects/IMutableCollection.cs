namespace Kagami.Library.Objects
{
   public interface IMutableCollection : ICollection
   {
      IObject Append(IObject obj);

      IObject Remove(IObject obj);

      IObject RemoveAt(int index);

      IObject InsertAt(int index, IObject obj);
   }
}