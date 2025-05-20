using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages
{
   public class RandomClass : BaseClass
   {
      public override string Name => "Random";

      public override void RegisterMessages()
      {
         base.RegisterMessages();
         collectionMessages();

         messages["next()"] = (obj, _) => function<XRandom>(obj, r => r.Next());
         messages["next(float:<Boolean>)"] =
            (obj, msg) => function<XRandom, KBoolean>(obj, msg, (r, b) => b.Value ? r.NextFloat() : r.Next());
         messages["next(max:<Int>)"] = (obj, msg) => function<XRandom, Int>(obj, msg, (r, i) => r.Next(i.Value));
         messages["next(min:<Int>,max:<Int>)"] =
            (obj, msg) => function<XRandom, Int, Int>(obj, msg, (r, i1, i2) => r.Next(i1.Value, i2.Value));
      }
   }
}