namespace Kagami.Library.Objects;

public class SkipAction(int count) : IStreamAction
{
   protected int index = -1;

   public ILazyStatus Next(ILazyStatus status) => ++index < count ? new Skipped() : status;

   public IEnumerable<IObject> Execute(IIterator iterator)
   {
      var i = -1;
      foreach (var value in iterator.List())
      {
         if (++i >= count)
         {
            yield return value;
         }
      }
   }

   public override string ToString() => $"skip {count}";
}