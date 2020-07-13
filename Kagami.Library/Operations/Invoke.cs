using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
	public class Invoke : OneOperandOperation
	{
		public static void InvokeInvokableObject(Machine machine, IInvokableObject invokableObject, Arguments arguments)
		{
			var invokable = invokableObject.Invokable;
			if (invokable is YieldingInvokable yi)
			{
				InvokeYieldingInvokable(machine, yi, arguments);
			}
			else
			{
				InvokeInvokable(machine, invokable, arguments,
					invokableObject is IProvidesFields pf && pf.ProvidesFields ? pf.Fields : new Fields());
			}
		}

		static void InvokeYieldingInvokable(Machine machine, YieldingInvokable invokable, Arguments arguments)
		{
			invokable.Arguments = arguments;
			var iterator = invokable.GetIterator(false);
			machine.Push((IObject)iterator);
		}

		public static void InvokeInvokable(Machine machine, IInvokable invokable, Arguments arguments, Fields fields)
		{
			if (invokable.Constructing)
			{
				InvokeConstructor(machine, invokable, arguments, fields);
			}
			else
			{
				var returnAddress = machine.Address + 1;
				var frame = new Frame(returnAddress, fields);
				machine.PushFrame(frame);
				frame = new Frame(arguments);
				frame.SetFields(invokable.Parameters);
				machine.PushFrame(frame);
				machine.GoTo(invokable.Address);
			}
		}

		public static void InvokeConstructor(Machine machine, IInvokable invokable, Arguments arguments, Fields fields)
		{
			var returnAddress = machine.Address + 1;
			var frame = new Frame(returnAddress, arguments, fields);
			machine.PushFrame(frame);
			frame.SetFields(invokable.Parameters);
			machine.GoTo(invokable.Address);
		}

		public static IMatched<IObject> InvokeObject(Machine machine, IObject value, Arguments arguments, ref bool increment)
		{
			switch (value)
			{
				case IInvokableObject io:
					InvokeInvokableObject(machine, io, arguments);
					increment = io.Invokable is YieldingInvokable;

					return notMatched<IObject>();
				case PackageFunction pf:
					increment = true;
					return pf.Invoke(arguments).Matched();
				case IMayInvoke mi:
					increment = true;
					return mi.Invoke(arguments.Value).Matched();
				case Pattern pattern:
					increment = true;
					var copy = pattern.Copy();
					copy.RegisterArguments(arguments);
					return copy.Matched<IObject>();
				case UserObject userObject:
               increment = true;
               return sendMessage(userObject, "invoke(_...)", arguments).Matched();
				default:
					return failedMatch<IObject>(incompatibleClasses(value, "Invokable object"));
			}
		}

		string fieldName;
		bool increment;

		public Invoke(string fieldName) => this.fieldName = fieldName;

		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			increment = false;
			if (value is Arguments arguments)
			{
				var image = fieldName;
				var ((isFound, field), (isFailure, exception)) = machine.Find(fieldName, true);
				if (!isFound && !isFailure)
				{
					var selector = arguments.Selector(fieldName);
					image = selector.Image;
					((isFound, field), (isFailure, exception)) = machine.Find(selector);
				}

				if (isFound && field != null)
				{
					return InvokeObject(machine, field.Value, arguments, ref increment);
				}
				else if (isFailure)
				{
					return failedMatch<IObject>(exception);
				}
				else
				{
					return failedMatch<IObject>(fieldNotFound(image));
				}
			}
			else
			{
				return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
			}
		}

		public override bool Increment => increment;

		public string FieldName => fieldName;

		public override string ToString() => $"invoke({fieldName})";
	}
}