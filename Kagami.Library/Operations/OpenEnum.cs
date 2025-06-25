using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class OpenEnum(string enumName) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (Module.Global.Value.Class(enumName) is (true, EnumClass enumClass))
      {
         enumClass.Open();
         return nil;
      }
      else
      {
         return classNotFound(enumName);
      }
   }

   public override string ToString() => $"open.enum({enumName})";
}