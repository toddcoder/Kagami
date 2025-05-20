namespace Kagami.Library.Objects
{
   public interface IObjectCompare
   {
      int Compare(IObject obj);

      IObject Object { get; }

      KBoolean Between(IObject min, IObject max, bool inclusive);

      KBoolean After(IObject min, IObject max, bool inclusive);
   }
}