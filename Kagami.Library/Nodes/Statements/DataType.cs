using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class DataType : Statement
   {
      string className;
      Hash<string, (IObject[], IObject)> comparisands;

      public DataType(string className, Hash<string, (IObject[], IObject)> comparisands)
      {
         this.className = className;
         this.comparisands = comparisands;
      }

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var item in comparisands)
         {
            var name = item.Key;
            (var data, var ordinal) = item.Value;

            var skipLabel = newLabel("skip");

            builder.FieldExists(name);
            builder.GoToIfTrue(skipLabel);

            if (data.Length == 0)
            {
               builder.NewField(name, false, true);
               builder.PushObject(new DataComparisand(className, name, data, ordinal));
               builder.AssignField(name, true);
            }
            else
            {
               var dataTypeCode = new DataTypeCode(className, name, data, ordinal);
               var block = new Block(dataTypeCode);
               var invokable =
                  new DataComparisandInvokable(name, new Parameters(data.Length), $"{name}({data.Select(d => d.Image).Listify()})");
               if (builder.RegisterInvokable(invokable, block, true).If(out var _, out var exception))
               {
                  builder.NewField(name, false, true);
                  builder.PushObject(new Lambda(invokable));
                  builder.AssignField(name, true);
               }
               else
                  throw exception;
            }

            builder.Label(skipLabel);
         }

         builder.NewField(className, false, true);
         builder.PushObject(new Objects.DataType(className, comparisands));
         builder.AssignField(className, true);
      }
   }
}