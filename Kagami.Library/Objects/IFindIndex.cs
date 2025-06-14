namespace Kagami.Library.Objects;

public interface IFindIndex
{
   IObject IndexOf(IObject value);

   IObject ReverseIndexOf(IObject value);

   IObject FindAll(Lambda predicate);

   IObject First(Lambda predicate);

   IObject Last(Lambda predicate);
}