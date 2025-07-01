using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class EnumClass : UserClass
{
   protected bool isEnumeration = true;
   protected SelectorHash<IObject> constructors = [];
   protected Hash<IObject, Selector> ordinalToSelector = [];

   public EnumClass(string className, string parentClassName) : base(className, parentClassName)
   {
   }

   public void RegisterMember(Selector constructorSelector, Selector messageSelector, Maybe<IObject> _ordinal)
   {
      if (isEnumeration && constructorSelector.SelectorItems.Length > 0)
      {
         isEnumeration = false;
      }

      constructors[constructorSelector] = _ordinal.Map(Some.Object) | None.NoneValue;
      RegisterClassMessage(messageSelector, (_, msg) => GetMember(constructorSelector, msg));

      var fieldName = constructorSelector.AsString.Substitute("^ /w+ '$' /(-['(']+) .* $", "`$1").ToLower1();
      registerClassMessage(fieldName.get(), (_, msg) => GetMember(constructorSelector, msg));

      if (_ordinal is (true, var ordinal))
      {
         ordinalToSelector[ordinal] = constructorSelector;
      }
   }

   public IObject GetMember(Selector constructorSelector, Message message)
   {
      if (constructors.ContainsKey(constructorSelector))
      {
         return createObject(constructorSelector, message);
      }
      else
      {
         throw classNotFound(className);
      }
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      registerClassMessage("values".get(), (_, _) => Values());
      registerClassMessage("fromOrdinal(_)", (_, msg) => FromOrdinal(msg.Arguments[0]));
   }

   public KTuple Values()
   {
      if (isEnumeration)
      {
         List<IObject> list = [];
         foreach (var (selector, value) in constructors)
         {
            if (value is None)
            {
               var message = new Message(selector, Arguments.Empty);
               var createdObject = createObject(selector, message);
               constructors[selector] = createdObject;
               list.Add(createdObject);
            }
            else
            {
               list.Add(value);
            }
         }

         return new KTuple([.. list]);
      }
      else
      {
         throw fail("Enumeration values only");
      }
   }

   protected IObject retrieveObject(Selector constructorSelector)
   {
      if (constructorSelector.SelectorItems.Length > 0)
      {
         return getConstructor(constructorSelector);
      }
      else
      {
         var message = new Message(constructorSelector, Arguments.Empty);
         return createObject(constructorSelector, message);
      }
   }

   public IObject FromOrdinal(IObject ordinal)
   {
      return ordinalToSelector.Maybe[ordinal].Map(retrieveObject).Map(Some.Object) | None.NoneValue;
   }

   public void Open()
   {
      StringHash<IObject> objects = [];
      foreach (var (selector, _) in constructors)
      {
         var name = $"`{selector.Substitute("^ /w+ '$' /(-['(']+) .* $", "$1").ToLower1()}";
         Selector constructorSelector = selector;
         objects[name] = retrieveObject(constructorSelector);
      }

      var machine = Machine.Current.Value;
      foreach (var (name, obj) in objects)
      {
         machine.CurrentFrame.Fields.New(name, obj);
      }
   }
}