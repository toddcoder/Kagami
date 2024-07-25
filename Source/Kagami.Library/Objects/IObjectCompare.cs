namespace Kagami.Library.Objects
{
   public interface IObjectCompare
   {
      int Compare(IObject obj);

      IObject Object { get; }

      Boolean Between(IObject min, IObject max, bool inclusive);

      Boolean After(IObject min, IObject max, bool inclusive);
   }
}