using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Collections;
using Core.Enumerables;
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
         foreach (var (key, value) in comparisands)
         {
	         var (data, ordinal) = value;

            var skipLabel = newLabel("skip");

            builder.FieldExists(key);
            builder.GoToIfTrue(skipLabel);

            if (data.Length == 0)
            {
               builder.NewField(key, false, true);
               builder.PushObject(new DataComparisand(className, key, data, ordinal));
               builder.AssignField(key, true);
            }
            else
            {
               var dataTypeCode = new DataTypeCode(className, key, data, ordinal);
               var block = new Block(dataTypeCode);
               var invokable =
                  new DataComparisandInvokable(key, new Parameters(data.Length), $"{key}({data.Select(d => d.Image).Stringify()})");
               if (builder.RegisterInvokable(invokable, block, true).If(out var _, out var exception))
               {
                  builder.NewField(key, false, true);
                  builder.PushObject(new Lambda(invokable));
                  builder.AssignField(key, true);
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