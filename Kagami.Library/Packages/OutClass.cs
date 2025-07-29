using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class OutClass : BaseClass
{
   public override string Name => "Out";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("<<(_)", (obj, msg) => function<Out, IObject>(obj, msg, (o, a) => o.Append(a)));
      registerMessage("<|(_)", (obj, msg) => function<Out, IObject>(obj, msg, (o, a) => o.AppendLine(a)));
   }

   public override IObject DefaultValue => throw noDefaultValue("Out");
}