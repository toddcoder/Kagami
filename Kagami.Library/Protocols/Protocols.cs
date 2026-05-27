using Core.Collections;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Protocols;

public static class Protocols
{
   private static StringHash<Protocol> protocols = [];

   public static Maybe<Protocol> Get(string protocolName) => protocols.Maybe[protocolName];

   public static Protocol GetOrThrow(string protocolName) => Get(protocolName).Required(messageProtocolNotFound(protocolName));

   public static void Set(string protocolName, Protocol protocol) => protocols[protocolName] = protocol;

   public static bool Supports(string protocolName, IObject obj)
   {
      if (protocols.Maybe[protocolName] is (true, var protocol))
      {
         return protocol.Supports(obj);
      }
      else
      {
         return false;
      }
   }

   public static bool Supports(string protocolName, BaseClass baseClass)
   {
      if (protocols.Maybe[protocolName] is (true, var protocol))
      {
         return protocol.Supports(baseClass);
      }
      else
      {
         return false;
      }
   }

   public static void Clear()
   {
      protocols.Clear();
      protocols["PError"] = new Protocol("PError", "message".get(), "callStack".get());

      Selector[] monadSelectors = [(Selector)"bind(_<Lambda>)", (Selector)"unit(_)"];
      protocols["PMonad"] = new Protocol("PMonad", monadSelectors);
      protocols["POptional"] = new Protocol("POptional", monadSelectors);
      protocols["PResult"] = new Protocol("PResult", monadSelectors);
   }
}