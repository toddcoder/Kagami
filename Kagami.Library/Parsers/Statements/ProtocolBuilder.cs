using Core.Collections;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Protocols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ProtocolBuilder(string protocolName)
{
   protected Set<Selector> selectors = [];

   public void AddSelector(Selector selector) => selectors.Add(selector);

   public void AddProtocol(Protocol otherProtocol)
   {
      foreach (var selector in otherProtocol.Selectors)
      {
         AddSelector(selector);
      }
   }

   public Optional<Protocol> Build()
   {
      var _protocol = Protocols.Protocols.Get(protocolName);
      if (_protocol)
      {
         return nil;
      }
      else
      {
         var protocol = new Protocol(protocolName, [.. selectors]);
         Protocols.Protocols.Set(protocolName, protocol);

         return protocol;
      }
   }
}