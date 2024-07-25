namespace Kagami.Library.Objects
{
   public interface ISkipTakeable
   {
      IObject this[SkipTake skipTake] { get; }
   }
}