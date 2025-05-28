using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class ErrorClass : BaseClass
{
   public override string Name => "Error";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["message".get()] = (obj, _) => function<Error>(obj, e => e.Message);
   }
}