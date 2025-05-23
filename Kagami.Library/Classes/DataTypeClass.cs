using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class DataTypeClass : UserClass
{
   protected LateLazy<DataType> dataType = new();
   protected Hash<string, IObject> dataComparisands = new();
   protected Hash<IObject, string> ordinals = new();

   public DataTypeClass(string className) : base(className, "")
   {
   }

   public void RegisterDataType(DataType dataType) => this.dataType.ActivateWith(() => dataType);

   public Result<Unit> RegisterDataComparisand(string name, IObject ordinal)
   {
      if (dataComparisands.ContainsKey(name))
      {
         return fail($"Data comparisand {name} already exists");
      }
      else
      {
         dataComparisands[name] = ordinal;
         ordinals[ordinal] = name;

         return unit;
      }
   }

   public override void RegisterMessage(Selector selector, Func<IObject, Message, IObject> func)
   {
      base.RegisterMessage(selector, func);

      RegisterDataComparisandMethod(selector, func);
   }

   public void RegisterDataComparisandMethod(string name, Func<IObject, Message, IObject> func)
   {
      foreach (var key in dataComparisands.Select(item => $"{name}.{item.Key}"))
      {
         if (Module.Global.Value.Class(key) is (true, var cls))
         {
            cls.RegisterMessage(key, func);
         }
      }
   }

   public override bool DynamicRespondsTo(Selector selector)
   {
      var name = selector.Name.StartsWith("__$") ? selector.Name.Drop(3) : selector.Name;
      return dataComparisands.ContainsKey(name);
   }

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      var dt = (DataType)obj;
      var name = message.Selector.Name.StartsWith("__$") ? message.Selector.Name.Drop(3) : message.Selector.Name;
      return dt.GetDataComparisand(name, message.Arguments.Value);
   }

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("fromOrdinal", (obj, msg) => function<DataType, IObject>(obj, msg, (_, ord) =>
      {
         if (ordinals.Maybe[ord] is (true, var name))
         {
            return new Some(dataType.Value.GetDataComparisand(name, msg.Arguments.Value.Skip(1).ToArray()));
         }
         else
         {
            return None.NoneValue;
         }
      }));
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      registerClassMessage("fromOrdinal", (bc, msg) => classFunc<DataTypeClass, IObject>(bc, msg, (td, ord) =>
      {
         if (td.ordinals.Maybe[ord] is (true, var name))
         {
            return new Some(dataType.Value.GetDataComparisand(name, msg.Arguments.Value.Skip(1).ToArray()));
         }
         else
         {
            return None.NoneValue;
         }
      }));

      RegisterClassMessage("values".get(), (bc, _) => classFunc<DataTypeClass>(bc, dtc => dtc.dataType.Value.Values));
   }

   public override bool ClassRespondsTo(Selector selector) => classMessages.ContainsKey(selector);

   public override bool AssignCompatible(BaseClass otherClass)
   {
      return base.AssignCompatible(otherClass) || dataComparisands.KeyArray().Any(k => $"{Name}.{k}" == otherClass.Name);
   }
}