using System;
using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class DataTypeClass : UserClass
   {
      protected DataType dataType;
      protected Hash<string, IObject> dataComparisands;
      protected Hash<IObject, string> ordinals;

      public DataTypeClass(string className) : base(className, "")
      {
         dataComparisands = new Hash<string, IObject>();
         ordinals = new Hash<IObject, string>();
      }

      public void RegisterDataType(DataType dataType) => this.dataType = dataType;

      public IResult<Unit> RegisterDataComparisand(string name, IObject ordinal)
      {
         if (dataComparisands.ContainsKey(name))
            return $"Data comparisand {name} already exists".Failure<Unit>();
         else
         {
            dataComparisands[name] = ordinal;
            ordinals[ordinal] = name;

            return Unit.Success();
         }
      }

      public override void RegisterMessage(string name, Func<IObject, Message, IObject> func)
      {
         base.RegisterMessage(name, func);

         RegisterDataComparisandMethod(name, func);
      }

      public void RegisterDataComparisandMethod(string name, Func<IObject, Message, IObject> func)
      {
         foreach (var item in dataComparisands)
         {
            var key = $"{name}.{item.Key}";
            if (Module.Global.Class(key).If(out var cls))
               cls.RegisterMessage(key, func);
         }
      }

      public override bool DynamicRespondsTo(string message)
      {
         var name = message.StartsWith("__$") ? message.Skip(3) : message;
         return dataComparisands.ContainsKey(name);
      }

      public override IObject DynamicInvoke(IObject obj, Message message)
      {
         var dt = (DataType)obj;
         var name = message.Name.StartsWith("__$") ? message.Name.Skip(3) : message.Name;
         return dt.GetDataComparisand(name, message.Arguments.Value);
      }

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerMessage("fromOrdinal", (obj, msg) => function<DataType, IObject>(obj, msg, (dt, ord) =>
         {
            if (ordinals.ContainsKey(ord))
            {
               var name = ordinals[ord];
               return new Some(dataType.GetDataComparisand(name, msg.Arguments.Value.Skip(1).ToArray()));
            }
            else
               return Nil.NilValue;
         }));
      }
   }
}