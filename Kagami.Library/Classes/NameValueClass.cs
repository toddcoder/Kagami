using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class NameValueClass : BaseClass
   {
      public override string Name => "NameValue";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["name".get()] = (obj, msg) => function<NameValue>(obj, nv => nv.Key);
         messages["value".get()] = (obj, msg) => function<NameValue>(obj, nv => nv.Value);
      }
   }
}