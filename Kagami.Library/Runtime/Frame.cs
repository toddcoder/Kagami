using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;
using Tuple = Kagami.Library.Objects.Tuple;

namespace Kagami.Library.Runtime
{
	public class Frame
	{
		public static Frame TryFrame() => new Frame() { FrameType = FrameType.Try };

		protected Stack<IObject> stack;
		protected IMaybe<IObject> returnValue;
		protected int address;
		protected Fields fields;
		protected Arguments arguments;
		protected FrameType frameType;
		protected bool parametersSet;

		public Frame(int address, Arguments arguments)
		{
			stack = new Stack<IObject>();
			returnValue = none<IObject>();
			this.address = address;
			fields = new Fields();
			this.arguments = arguments;
			frameType = FrameType.Function;
			parametersSet = false;
		}

		public Frame(int address, Fields fields)
		{
			stack = new Stack<IObject>();
			returnValue = none<IObject>();
			this.address = address;
			this.fields = fields;
			arguments = Arguments.Empty;
			frameType = FrameType.Function;
			parametersSet = false;
		}

		public Frame(int address, Arguments arguments, Fields fields)
		{
			stack = new Stack<IObject>();
			returnValue = none<IObject>();
			this.address = address;
			this.fields = fields;
			this.arguments = arguments;
			frameType = FrameType.Function;
			parametersSet = false;
		}

		public Frame(int address, IInvokable invokable)
		{
			stack = new Stack<IObject>();
			returnValue = none<IObject>();
			this.address = address;
			if (invokable is IProvidesFields pf && pf.ProvidesFields)
			{
				fields = pf.Fields;
			}
			else
			{
				fields = new Fields();
			}

			arguments = Arguments.Empty;
			frameType = FrameType.Function;
			parametersSet = false;
		}

		public Frame() : this(-1, Arguments.Empty) => frameType = FrameType.Standard;

		public Frame(Fields fields) : this(-1, fields) => frameType = FrameType.Standard;

		public Frame(IInvokable invokable) : this(-1, invokable) => frameType = FrameType.Standard;

		public Frame(Arguments arguments) : this(-1, arguments) => frameType = FrameType.Standard;

		public FrameType FrameType
		{
			get => frameType;
			set => frameType = value;
		}

		public void SetFields(Parameters parameters)
		{
			if (!parametersSet)
			{
				var length = Math.Min(arguments.Length, parameters.Length);
				var lastValue = Unassigned.Value;
				var lastName = "";
				var variadic = parameters.Length > 0 && parameters[0].Variadic;

				if (variadic)
				{
					var parameter = parameters[0];
					var tuple = new Tuple(arguments.ToArray());
					if (!fields.ContainsKey(parameter.Name))
					{
						fields.New(parameter.Name, parameter.Mutable).Force();
					}

					if (parameter.TypeConstraint.If(out var typeConstraint) && !typeConstraint.Matches(classOf(lastValue)))
					{
						throw incompatibleClasses(lastValue, typeConstraint.AsString);
					}

					fields.Assign(parameter.Name, tuple, true).Force();
					return;
				}

				for (var i = 0; i < length && !variadic; i++)
				{
					var parameter = parameters[i];
					lastValue = arguments[i];
					if (!fields.ContainsKey(parameter.Name))
					{
						fields.New(parameter.Name, parameter.Mutable).Force();
					}

					if (parameter.TypeConstraint.If(out var typeConstraint) && !typeConstraint.Matches(classOf(lastValue)))
					{
						throw incompatibleClasses(lastValue, typeConstraint.AsString);
					}

					fields.Assign(parameter.Name, lastValue, true).Force();
					lastName = parameter.Name;
					variadic = parameter.Variadic;
				}

				if (variadic)
				{
					var tupleList = new List<IObject> { lastValue };
					for (var i = length; i < arguments.Length; i++)
					{
						tupleList.Add(arguments[i]);
					}

					var tuple = new Tuple(tupleList.ToArray());
					fields.Assign(lastName, tuple, true).Force();
				}
				else if (length < parameters.Length)
				{
					for (var i = length; i < parameters.Length; i++)
					{
						var parameter = parameters[i];
						var defaultValue = parameter.DefaultValue;
						if (!fields.ContainsKey(parameter.Name))
						{
							fields.New(parameter.Name, parameter.Mutable).Force();
						}

						IObject value;
						if (defaultValue.If(out var invokable))
						{
							if (Machine.Current.Invoke(invokable, Arguments.Empty, 0).If(out value, out var anyException)) { }
							else if (anyException.If(out var exception))
							{
								throw exception;
							}
						}
						else
						{
							value = Unassigned.Value;
						}

						if (parameter.TypeConstraint.If(out var typeConstraint) && !typeConstraint.Matches(classOf(value)))
						{
							throw incompatibleClasses(value, typeConstraint.AsString);
						}

						fields.Assign(parameter.Name, value, true).Force();
					}
				}
				else if (length < arguments.Length)
				{
					var tupleList = new List<IObject> { lastValue };
					for (var i = length; i < arguments.Length; i++)
					{
						tupleList.Add(arguments[i]);
					}

					var tuple = new Tuple(tupleList.ToArray());
					fields.Assign(lastName, tuple, true).Force();
				}

				parametersSet = true;
			}
		}

		public void Push(IObject value) => stack.Push(value);

		public bool IsEmpty => stack.Count == 0;

		public IMaybe<IObject> Peek() => maybe(stack.Count > 0, () => stack.Peek());

		public IResult<IObject> Pop() => tryTo(() => stack.Pop());

		public void SetReturnValue(IObject value) => returnValue = value.Some();

		public IMaybe<IObject> ReturnValue => returnValue;

		public int Address
		{
			get => address;
			set => address = value;
		}

		public Fields Fields => fields;

		public Arguments Arguments => arguments;

		public void Clear() => stack.Clear();

		public override string ToString()
		{
			return (StringStream)"(" / stack.Select(v => v.Image).ToString(", ") / ")[" / fields.FieldNames.ToString(", ") / "]";
		}

		public void CopyFromFields(Fields sourceFields) => fields.CopyFrom(sourceFields);

		public IMaybe<int> ErrorHandler { get; set; } = none<int>();

		public IMaybe<Unit> Swap(int index)
		{
			var index2 = index + 1;
			if (index2 < stack.Count)
			{
				var array = stack.ToArray();
				var temp = array[index];
				array[index] = array[index2];
				array[index2] = temp;
				stack = new Stack<IObject>(array);

				return Unit.Some();
			}
			else
			{
				return none<Unit>();
			}
		}

		public IMaybe<IObject> Pick(int index)
		{
			if (index < stack.Count)
			{
				var array = stack.ToArray();
				var item = array[index];
				var list = array.ToList();
				list.RemoveAt(index);
				list.Reverse();
				stack = new Stack<IObject>(list);

				return item.Some();
			}
			else
			{
				return none<IObject>();
			}
		}

		public IMaybe<IObject> Copy(int index)
		{
			if (index < stack.Count)
			{
				var list = stack.ToList();
				var item = list[index];
				list.Reverse();
				stack = new Stack<IObject>(list);

				return item.Some();
			}
			else
			{
				return none<IObject>();
			}
		}
	}
}