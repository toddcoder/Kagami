using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Join(bool cumulative) : TwoOperandOperation
{
   protected KString join(IIterator iterator, string joinOn)
   {
      if (cumulative)
      {
         var expression = "";
         var joinOperator = $" {joinOn} ";
         List<string> list = [];
         foreach (var item in iterator.List())
         {
            if (expression.IsNotEmpty())
            {
               expression = $"{expression}{joinOperator}{item.Image}";
               list.Add(expression);
            }
            else
            {
               expression = item.Image;
               list.Add(expression);
            }
         }

         return $"[{list.ToString(", ")}]";
      }
      else
      {
         return iterator.List().Select(i => i.Image).ToString(joinOn);
      }
   }

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      IIterator iterator => join(iterator, y.AsString),
      ICollection collection => join(collection.GetIterator(false), y.AsString),
      _ => expectedType("Iterator or Collection")
   };

   public override string ToString() => "join";
}