using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class KeyValueClass : BaseClass
   {
      public override string Name => "KeyValue";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["key".get()] = (obj, _) => function<KeyValue>(obj, kv => kv.Key);
         messages["value".get()] = (obj, _) => function<KeyValue>(obj, kv => kv.Value);
      }
   }
}