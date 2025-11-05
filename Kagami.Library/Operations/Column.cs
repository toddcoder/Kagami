using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Column : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Sequence sequence)
      {
         var list = sequence.List;
         if (list.Count == 2)
         {
            var column0 = list[0];
            var column1 = list[1];

            if (column1 is Int columnInt)
            {
               var text = column0.AsString;
               var columnIndex = columnInt.Value;
               machine.Context.Put(text);
               if (machine.Context.WriteCount == columnIndex - 1)
               {
                  machine.Context.PrintLine("");
               }

               if (columnIndex > 0)
               {
                  machine.Context.WriteCount = (machine.Context.WriteCount + 1) % columnIndex;
               }

               var image = (KString)value.Image;
               return image;
            }
            else
            {
               return fail("Column isn't an Int");
            }
         }
         else
         {
            return fail("There must be 2 arguments");
         }
      }
      else
      {
         return fail("There must be 2 arguments");
      }
   }

   public override string ToString() => "column";
}