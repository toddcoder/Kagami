using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Exceptions;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Classes
{
	public class UserClass : BaseClass
	{
		protected string className;
		protected string parentClassName;
		protected IMaybe<UserClass> parentClass;
		protected Set<Selector> signatures;
		protected IMaybe<UserObject> metaObject;

		public UserClass(string className, string parentClassName)
		{
			this.className = className;
			this.parentClassName = parentClassName;

			if (this.parentClassName.IsNotEmpty())
				parentClass = Module.Global.Class(parentClassName).Map(bc => (UserClass)bc);
			else
				parentClass = none<UserClass>();

			signatures = new Set<Selector>();

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
			foreach (var (key, value) in parentClass.messages)
				messages[key] = value;
			foreach (var selector in parentClass.signatures)
				signatures.Add(selector);
		}

		public virtual bool RegisterMethod(Selector selector, Lambda lambda, bool overriding)
		{
			if (messages.ContainsExact(selector) && !overriding)
				return false;
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
				Selector getter = name.get();
				messages[getter] = (obj, msg) => ((UserObject)obj).Fields[name];
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
			registerMessage("className".get(), (obj, msg) => String.StringObject(obj.ClassName));
			registerMessage("class".get(), (obj, msg) => new Class(obj.ClassName));
			registerMessage("send",
				(obj, msg) => function<IObject, String>(obj, msg, (o, n) => sendMessage(o, n.Value, msg.Arguments.Pass(1))));
		}

		public IMatched<Selector> MatchImplemented(IEnumerable<Selector> traitSignatures)
		{
			foreach (var signature in traitSignatures)
				if (!signatures.Contains(signature))
					return signature.Matched();

			return notMatched<Selector>();
		}

		public override void RegisterClassMessages() { }

		public override bool ClassRespondsTo(Selector selector)
		{
			return metaObject.FlatMap(uo => classOf(uo).RespondsTo(selector), () => false);
		}

		public override IObject ClassDynamicInvoke(Message message)
		{
			if (metaObject.If(out var uo))
				return sendMessage(uo, message);
			else
				throw "No metaobject".Throws();
		}

		public override bool RespondsTo(Selector selector)
		{
			if (base.RespondsTo(selector))
				return true;
			else
				return messages.ContainsKey("missing(_<String>,_<Tuple>)");
		}

		public override IObject DynamicInvoke(IObject obj, Message message)
		{
			var originalMessage = String.StringObject(message.Selector.Name);
			var args = message.Arguments.ToArray();
			var tuple = new Tuple(args);

			return sendMessage(obj, "missing(_<String>,_<Tuple>)", originalMessage, tuple);
		}

		public override bool AssignCompatible(BaseClass otherClass)
		{
			if (Name == otherClass.Name)
				return true;
			else if (parentClass.If(out var pc))
				return pc.AssignCompatible(otherClass);
			else
				return false;
		}

		public override bool MatchCompatible(BaseClass otherClass)
		{
			if (Name == otherClass.Name)
				return true;
			else if (parentClass.If(out var pc))
				return pc.MatchCompatible(otherClass);
			else
				return false;
		}
	}
}