namespace Kagami.Library.Objects;

public interface IFindIndex
{
   IObject IndexOf(IObject value);

   IObject Index(Lambda predicate);

   IObject LastIndex(Lambda predicate);

   IObject LastIndexOf(IObject value);

   IObject FindAll(Lambda predicate);

   IObject First(Lambda predicate);

   IObject Last(Lambda predicate);

   IObject BinarySearch(IObject item);

   IObject BinarySearch(IObject item, Lambda lambda);
}