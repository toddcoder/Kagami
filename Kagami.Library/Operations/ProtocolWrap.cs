using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class ProtocolWrap(string protocolName) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (Protocols.Protocols.Get(protocolName) is (true, var protocol))
      {
         if (protocol.Supports(value))
         {
            return new ProtocolWrapper(value, protocol);
         }
         else
         {
            return protocolNotImplemented(value.ClassName, protocolName);
         }
      }
      else
      {
         return protocolNotFound(protocolName);
      }
   }

   public override string ToString() => $"protocol.wrap({protocolName})";
}