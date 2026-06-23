using Core.Collections;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
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

   public static Result<Unit> Create(string protocolName, MetaClass metaClass)
   {
      try
      {
         var statements = metaClass.ClassBuilder.Statements;
         Set<Selector> selectors = [];

         foreach (var statement in statements)
         {
            switch (statement)
            {
               case IFieldStatement fieldStatement:
               {
                  selectors.AddRange(fieldStatement.Selectors());
                  break;
               }
               case IFieldsStatement fieldsStatement:
               {
                  foreach (var fieldStatement in fieldsStatement.FieldStatements())
                  {
                     selectors.AddRange(fieldStatement.Selectors());
                  }

                  break;
               }
               case IHasSelector hasSelector:
               {
                  selectors.Add(hasSelector.Selector);
                  break;
               }
            }
         }

         var protocol = new Protocol(protocolName, [.. selectors]);
         Set(protocolName, protocol);

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}