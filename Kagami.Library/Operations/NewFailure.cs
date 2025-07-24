using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewFailure : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      KString kString => new Objects.Failure(kString.Value),
      Error error => new Objects.Failure(error),
      Objects.Failure failure => failure,
      _ => incompatibleClasses(value, "String or Error")
   };

   public override string ToString() => "new.failure";
}