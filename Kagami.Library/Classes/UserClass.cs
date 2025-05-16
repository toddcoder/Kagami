using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Classes;

public class UserClass : BaseClass
{
   protected string className;
   protected string parentClassName;
   protected Maybe<UserClass> _parentClass;
   protected Set<Selector> signatures = [];
   protected Maybe<UserObject> _metaObject = nil;
   protected SelectorHash<UserClass> mixins = [];

   public UserClass(string className, string parentClassName)
   {
      this.className = className;
      this.parentClassName = parentClassName;

      _parentClass = maybe<UserClass>() & this.parentClassName.IsNotEmpty() &
         (() => Module.Global.Class(parentClassName).Map(bc => (UserClass)bc));
   }

   public void Include(Mixin mixin)
   {
      var mixinClass = (UserClass)classOf(mixin);
      foreach (var (selector, _) in mixinClass.messages)
      {
         mixins[selector] = mixinClass;
      }
   }

   public bool Includes(string mixinClassName) => mixins.Values.Select(uc => uc.Name).Any(n => n == mixinClassName);

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

   public virtual bool RegisterMethod(Selector selector, Lambda lambda, bool overriding)
   {
      if (messages.ContainsExact(selector) && !overriding)
      {
         return false;
      }
      else
      {
         var clone = lambda.Clone();
         messages[selector] = (obj, msg) => Invoke((UserObject)obj, msg.Arguments, clone);
         signatures.Add(selector);

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
      foreach (var parameter in parameters)
      {
         var name = parameter.Name;
         Selector getter = name.get();
         messages[getter] = (obj, _) => ((UserObject)obj).Fields[name];
         signatures.Add(getter);
         if (parameter.Mutable)
         {
            Selector setter = name.set();
            messages[setter] = (obj, msg) => ((UserObject)obj).Fields[name] = msg.Arguments[0];
            signatures.Add(setter);
         }
      }
   }

   public override void RegisterMessages()
   {
      registerMessage("className".get(), (obj, _) => String.StringObject(obj.ClassName));
      registerMessage("class".get(), (obj, _) => new Class(obj.ClassName));
      registerMessage("send",
         (obj, msg) => function<IObject, String>(obj, msg, (o, n) => sendMessage(o, n.Value, msg.Arguments.Pass(1))));
   }

   public Optional<Selector> MatchImplemented(IEnumerable<Selector> traitSignatures)
   {
      foreach (var signature in traitSignatures)
      {
         if (!signatures.Contains(signature))
         {
            return signature;
         }
      }

      return nil;
   }

   public override void RegisterClassMessages()
   {
   }

   public override bool ClassRespondsTo(Selector selector) => _metaObject.Map(uo => classOf(uo).RespondsTo(selector)) | false;

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

   public override bool RespondsTo(Selector selector)
   {
      if (base.RespondsTo(selector))
      {
         return true;
      }
      else if (mixins.ContainsKey(selector))
      {
         return true;
      }
      else
      {
         return messages.ContainsKey("missing(_<String>,_<Tuple>)");
      }
   }

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      if (mixins.ContainsKey(message.Selector))
      {
         return mixins[message.Selector].SendMessage(obj, message);
      }
      else
      {
         var originalMessage = String.StringObject(message.Selector.Name);
         var args = message.Arguments.ToArray();
         var tuple = new Tuple(args);

         return sendMessage(obj, "missing(_<String>,_<Tuple>)", originalMessage, tuple);
      }
   }

   public override bool AssignCompatible(BaseClass otherClass)
   {
      if (Name == otherClass.Name)
      {
         return true;
      }
      else if (_parentClass is (true, var parentClass))
      {
         return parentClass.AssignCompatible(otherClass);
      }
      else
      {
         return false;
      }
   }

   public override bool MatchCompatible(BaseClass otherClass)
   {
      if (Name == otherClass.Name)
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
}