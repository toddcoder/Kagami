using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class FlexRangeClass : BaseClass
   {
      public override string Name => "FlexRange";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         collectionMessages();

         messages["from".get()] = (obj, msg) => function<FlexRange>(obj, fr => fr.From);
         messages["by".get()] = (obj, msg) => function<FlexRange>(obj, fr => fr.By);
         messages["to".get()] = (obj, msg) => function<FlexRange>(obj, fr => fr.To);
         messages["inclusive".get()] = (obj, msg) => function<FlexRange>(obj, fr => fr.Inclusive);
      }
   }
}