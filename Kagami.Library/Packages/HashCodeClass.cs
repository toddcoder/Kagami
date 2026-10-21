using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class HashCodeClass : BaseClass
{
   public override string Name => "HashCode";

   public override IObject DefaultValue => new HashCode();

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["<<(_)"] = (obj, msg) => function<HashCode, IObject>(obj, msg, (h, v) => h.Add(v));
   }
}