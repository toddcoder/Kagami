using Core.Collections;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Protocols;

public class Protocol(string name, params Selector[] selectors)
{
   public static ProtocolResult FromObject(IObject obj, string protocolName)
   {
      try
      {
         if (Protocols.Get(protocolName) is (true, var protocol))
         {
            List<Selector> missingSelectors = [];
            var @class = classOf(obj);

            foreach (var selector in protocol.Selectors)
            {
               if (!@class.RespondsTo(selector))
               {
                  missingSelectors.Add(selector);
               }
            }

            if (missingSelectors.Count == 0)
            {
               return new ProtocolResult.Found(protocol);
            }
            else
            {
               return new ProtocolResult.Missing(missingSelectors);
            }
         }
         else
         {
            return new ProtocolResult.NotFound();
         }
      }
      catch (Exception exception)
      {
         return new ProtocolResult.Error(exception);
      }
   }

   public string Name => name;

   public Selector[] Selectors => selectors;

   public IObject SendMessage(IObject obj, Selector selector, params IObject[] arguments)
   {
      if (selectors.Contains(selector))
      {
         return sendMessage(obj, selector, arguments);
      }
      else
      {
         throw unsupportedByProtocol(name, selector);
      }
   }

   public IObject SendMessage(IObject obj, Message message)
   {
      var messageSelector = message.Selector;
      if (selectors.Contains(messageSelector))
      {
         return sendMessage(obj, message);
      }
      else
      {
         throw unsupportedByProtocol(name, messageSelector);
      }
   }

   public bool Supports(IObject obj)
   {
      var @class = classOf(obj);
      return selectors.All(@class.RespondsTo);
   }

   public bool Supports(IEnumerable<Selector> includedSelectors)
   {
      Set<Selector> includedSet = [.. includedSelectors];
      foreach (var selector in selectors)
      {
         if (!includedSet.Contains(selector))
         {
            return false;
         }
      }

      return true;
   }
}