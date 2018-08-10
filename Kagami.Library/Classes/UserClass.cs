using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Exceptions;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Classes
{
   public class UserClass : BaseClass
   {
      protected string className;
      protected string parentClassName;
      protected IMaybe<UserClass> parentClass;
      protected Hash<string, Signature> signatures;
      protected IMaybe<UserObject> metaObject;

      public UserClass(string className, string parentClassName)
      {
         this.className = className;
         this.parentClassName = parentClassName;

         if (this.parentClassName.IsNotEmpty())
            parentClass = Module.Global.Class(parentClassName).Map(bc => (UserClass)bc);
         else
            parentClass = none<UserClass>();

         signatures = new Hash<string, Signature>();

         metaObject = none<UserObject>();
      }

      public override string Name => className;

      public string ParentClassName => parentClassName;

      public IMaybe<UserClass> ParentClass => parentClass;

      public override bool UserDefined => true;

      public IMaybe<UserObject> MetaObject
      {
         get => metaObject;
         set => metaObject = value;
      }

      public void InheritFrom(UserClass parentClass)
      {
         foreach (var item in parentClass.messages)
            messages[item.Key] = item.Value;
         foreach (var item in parentClass.signatures)
            signatures[item.Key] = item.Value;
      }

      public virtual bool RegisterMethod(string fullFunctionName, Lambda lambda, bool overriding)
      {
         if (messages.ContainsKey(fullFunctionName) && !overriding)
            return false;
         else
         {
            var clone = lambda.Clone();
            messages[fullFunctionName] = (obj, msg) => Invoke((UserObject)obj, msg.Arguments, clone);
            signatures[fullFunctionName] = new Signature(fullFunctionName, clone.Invokable.Parameters.Length);

            return true;
         }
      }

      public void RegisterFields(Fields fields)
      {
         foreach (var item in fields)
         {
            var (fieldName, field) = item;
            if (fieldName.StartsWith("__$") || field.Value is IInvokableObject)
               continue;

            var getter = fieldName.get();
            messages[getter] = (obj, msg) => ((UserObject)obj).Fields[fieldName];
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
            var getter = name.get();
            messages[getter] = (obj, msg) => ((UserObject)obj).Fields[name];
            signatures[getter] = new Signature(getter, 0);
            if (parameter.Mutable)
            {
               var setter = name.set();
               messages[setter] = (obj, msg) => ((UserObject)obj).Fields[name] = msg.Arguments[0];
               signatures[setter] = new Signature(setter, 1);
            }
         }
      }

      public override void RegisterMessages()
      {
         registerMessage("send",
            (obj, msg) => function<IObject, String>(obj, msg, (o, n) => sendMessage(o, n.Value, msg.Arguments.Pass(1))));
      }

      public IMatched<Signature> MatchImplemented(IEnumerable<Signature> traitSignatures)
      {
         foreach (var signature in traitSignatures)
            if (signatures.ContainsKey(signature.FullFunctionName))
            {
               var localSignature = signatures[signature.FullFunctionName];
               if (localSignature.ParameterCount != signature.ParameterCount)
                  return $"Signature for {signature.FullFunctionName} requires {signature.ParameterCount} parameters"
                     .FailedMatch<Signature>();
            }
            else
               return signature.Matched();

         return notMatched<Signature>();
      }

      public override void RegisterClassMessages() { }

      public override bool ClassRespondsTo(string message)
      {
         return metaObject.FlatMap(uo => classOf(uo).RespondsTo(message), () => false);
      }

      public override IObject ClassDynamicInvoke(Message message)
      {
         if (metaObject.If(out var uo))
            return sendMessage(uo, message);
         else
            throw "No metaobject".Throws();
      }

      public override bool RespondsTo(string message)
      {
         if (base.RespondsTo(message))
            return true;
         else
            return messages.ContainsKey("missing");
      }

      public override IObject DynamicInvoke(IObject obj, Message message)
      {
         var originalMessage = String.StringObject(message.Name);
         var args = message.Arguments.ToArray();
         var tuple = new Tuple(args);

         return sendMessage(obj, "missing", originalMessage, tuple);
      }
   }
}