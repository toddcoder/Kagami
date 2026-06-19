using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Protocols;

public class Protocol(string name, params SelectorWithType[] selectors)
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

   public SelectorWithType[] Selectors => selectors;

   public IObject SendMessage(IObject obj, SelectorWithType selector, params IObject[] arguments)
   {
      if (selectors.Contains(selector))
      {
         return sendMessage(obj, selector.Selector, arguments);
      }
      else
      {
         throw unsupportedByProtocol(name, selector.Selector);
      }
   }

   public IObject SendMessage(IObject obj, Message message)
   {
      var messageSelector = message.Selector;
      if (selectors.Select(s => s.Selector).Contains(messageSelector))
      {
         return sendMessage(obj, message);
      }
      else
      {
         throw unsupportedByProtocol(name, messageSelector);
      }
   }

   public bool Supports(BaseClass baseClass) => selectors.All(s => baseClass.RespondsTo(s.Selector));

   public bool Supports(IObject obj)
   {
      var @class = classOf(obj);
      return Supports(@class);
   }

   public Maybe<string> Supports(IEnumerable<Selector> includedSelectors)
   {
      Set<Selector> includedSet = [.. includedSelectors];
      List<string> missing = [];
      foreach (var selector in selectors)
      {
         if (!includedSet.Contains(selector))
         {
            missing.Add(selector.AsString);
         }
      }

      return maybe<string>() & missing.Count > 0 & (() => missing.ToString(", "));
   }
}