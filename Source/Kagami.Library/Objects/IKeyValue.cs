namespace Kagami.Library.Objects
{
   public interface IKeyValue
   {
      IObject Key { get; }

      IObject Value { get; }

      bool ExpandInTuple { get; }
   }
}