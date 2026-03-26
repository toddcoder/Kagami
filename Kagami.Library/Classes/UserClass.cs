using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using Class = Kagami.Library.Objects.Class;

namespace Kagami.Library.Classes;

public class UserClass : BaseClass, IEquatable<UserClass>
{
   protected string className;
   protected string parentClassName;
   protected Maybe<UserClass> _parentClass;
   protected Set<Selector> signatures = [];
   protected Maybe<UserObject> _metaObject = nil;
   protected StringHash<IObject> delegates = [];

   public UserClass(string className, string parentClassName)
   {
      this.className = className;
      this.parentClassName = parentClassName;

      _parentClass = maybe<UserClass>() & this.parentClassName.IsNotEmpty() &
         (() => Module.Global.Value.Class(parentClassName).Map(bc => (UserClass)bc));
   }

   public override string Name => className;

   public string ParentClassName => parentClassName;

   public Maybe<UserClass> ParentClass => _parentClass;

   public override bool UserDefined => true;

   public Maybe<UserObject> MetaObject
   {
      get => _metaObject;
      set => _metaObject = value;
   }

   public void InheritFrom(UserClass parentClass)
   {
      foreach (var (key, value) in parentClass.messages)
      {
         messages[key] = value;
      }

      foreach (var selector in parentClass.signatures)
      {
         signatures.Add(selector);
      }
   }

   public static IObject UserInvoke(UserObject userObject, Arguments arguments, Lambda lambda)
   {
      var machine = Machine.Current;
      lambda.Invokable.Class = new Class(userObject.ClassName);
      var _value = machine.Invoke(lambda.Invokable, arguments, userObject.Fields, false, true);
      if (_value is (true, var value))
      {
         return value;
      }
      else if (_value.Exception is (true, var exception))
      {
         throw exception;
      }
      else
      {
         return KUnit.Value;
      }
   }

   public virtual bool RegisterMethod(Selector selector, Lambda lambda, bool overriding)
   {
      if (messages.ContainsExact(selector) && !overriding)
      {
         return false;
      }
      else
      {
         var clone = lambda.Clone();
         Func<IObject, Message, IObject> invocation = (obj, msg) => UserInvoke((UserObject)obj, msg.Arguments, clone);
         foreach (var subSelector in selector.AllSelectors())
         {
            messages[subSelector] = invocation;
            signatures.Add(subSelector);
         }

         //signatures.Add(selector);

         return true;
      }
   }

   public void RegisterFields(Fields fields)
   {
      foreach (var item in fields)
      {
         var (fieldName, field) = item;
         if (fieldName.StartsWith("__$") || field.Value is IInvokableObject)
         {
            continue;
         }

         var getter = fieldName.get();
         messages[getter] = (obj, _) => ((UserObject)obj).Fields[fieldName];
         if (field.Mutable)
         {
            var setter = fieldName.set();
            messages[setter] = (obj, msg) => ((UserObject)obj).Fields[fieldName] = msg.Arguments[0];
         }
      }
   }

   public void RegisterParameters(Parameters parameters)
   {
      foreach (var parameter in parameters.Where(p => !p.IsHidden))
      {
         var name = parameter.Name;
         Selector getter = name.get();
         messages[getter] = (obj, _) => ((UserObject)obj).Fields[name];
         signatures.Add(getter);
         if (parameter.Mutable)
         {
            Selector setter = name.set();
            messages[setter] = (obj, msg) =>
            {
               ((UserObject)obj).Fields[name] = msg.Arguments[0];
               return msg.Arguments[0];
            };
            signatures.Add(setter);
         }
      }
   }

   public override void RegisterMessages()
   {
      registerMessage("className".get(), (obj, _) => KString.StringObject(obj.ClassName));
      registerMessage("class".get(), (obj, _) => new Class(obj.ClassName));
      registerMessage("class()", (obj, _) => new Class(obj.ClassName));
      registerMessage("send(_<String>,_...)",
         (obj, message) => function<IObject, KString>(obj, message, (o, n) => sendMessage(o, n.Value, message.Arguments.Pass(1))));
      registerMessage("send(_<String>)",
         (obj, message) => function<IObject, KString>(obj, message, (o, n) => sendMessage(o, n.Value, Arguments.Empty)));
      registerMessage("with(_)", (obj, msg) => ((UserObject)obj).With(msg.Arguments[0]));
      registerMessage("copy()", (obj, _) => ((UserObject)obj).Copy());
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      registerClassMessage("class()", (bc, _) => new Class(bc.Name));
   }

   public override bool ClassRespondsTo(Selector selector) =>
      _metaObject.Map(uo => classOf(uo).RespondsTo(selector)) | (() => base.ClassRespondsTo(selector));

   public override IObject ClassDynamicInvoke(Message message)
   {
      if (_metaObject is (true, var metaObject))
      {
         return sendMessage(metaObject, message);
      }
      else
      {
         throw fail("No metaobject");
      }
   }

   public override bool AssignCompatible(BaseClass otherClass)
   {
      if (otherClass.Name == "Placeholder")
      {
         return true;
      }
      else if (Name == otherClass.Name)
      {
         return true;
      }
      else if (_parentClass is (true, var parentClass))
      {
         if (parentClass.AssignCompatible(otherClass))
         {
            return true;
         }
         else if (otherClass is UserClass { ParentClass: (true, var otherParentClass) })
         {
            return parentClass.AssignCompatible(otherParentClass);
         }
         else
         {
            return false;
         }
      }
      else
      {
         return false;
      }
   }

   public override IObject DefaultValue => throw noDefaultValue(Name);

   public override bool MatchCompatible(BaseClass otherClass)
   {
      if (otherClass.Name == "Placeholder")
      {
         return true;
      }
      else if (Name == otherClass.Name)
      {
         return true;
      }
      else if (_parentClass is (true, var parentClass))
      {
         return parentClass.MatchCompatible(otherClass);
      }
      else
      {
         return false;
      }
   }

   public void RegisterDelegate(string className, IObject obj) => delegates[className] = obj;

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      foreach (var (_, @delegate) in delegates)
      {
         var delegateClass = classOf(@delegate);
         if (delegateClass.RespondsTo(message.Selector))
         {
            return delegateClass.SendMessage(@delegate, message);
         }
      }

      return base.DynamicInvoke(obj, message);
   }

   public bool Equals(UserClass? other) => base.Equals(other) && className == other.className;

   public override bool Equals(object? obj) => obj is UserClass otherUserClass && Equals(otherUserClass);

   public override int GetHashCode() => className.GetHashCode();

   public static bool operator ==(UserClass? left, UserClass? right) => Equals(left, right);

   public static bool operator !=(UserClass? left, UserClass? right) => !Equals(left, right);
}